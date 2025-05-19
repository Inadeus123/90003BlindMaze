using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarTrailSpawn : MonoBehaviour
{
    public GameObject carTrailEndPos; // Cartrail endpoint prefab
    public List<GameObject> spawnCars = new List<GameObject>();
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 1.5f;
    [SerializeField, ReadOnly] private float carHorizontalOffset;
    public Vector2 spawnAreaSize = new Vector2(6f,3f);
    public float spawnOffsetZ = 0f;
    
    private float timer = 0.0f;
    private float spawnInterval;

    private void Start()
    {
        // Set the initial random spawn interval
        GenerateRandomSpawnInterval();
    }

    private void Update()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Check if it's time to spawn a new object
        if (timer >= spawnInterval)
        {
            // Reset the timer
            timer = 0.0f;

            // Spawn a random object from the list
            SpawnRandomObject();

            // Generate a new random spawn interval
            GenerateRandomSpawnInterval();
        }
    }

    public void SpawnCar()
    {
        if (spawnCars.Count == 0) return;

        // 起点横截面内随机位置
        float offsetX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float offsetY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
        Vector3 spawnPos = transform.position + new Vector3(offsetX, offsetY, spawnOffsetZ);

        GameObject prefab = spawnCars[Random.Range(0, spawnCars.Count)];
        GameObject car = Instantiate(prefab, spawnPos, Quaternion.identity);
        //car.gameObject.transform.parent = transform;
        car.tag = "Car";
        CarMover mover = car.GetComponent<CarMover>();
        if (mover != null)
        {
            Vector3 mappingEndPos = carTrailEndPos.transform.position + new Vector3(offsetX, offsetY, spawnOffsetZ);
            mover.moveDir = (mappingEndPos - spawnPos).normalized;
        }
    }

    public GameObject GetRandomCarFromCarList()
    { 
        return spawnCars[Random.Range(0, spawnCars.Count)];
    }
    
    private void GenerateRandomSpawnInterval()
    {
        // Calculate a random spawn interval between the minimum and maximum values
        spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void SpawnRandomObject()
    {
        // Check if the object list is empty
        if (spawnCars.Count == 0)
        {
            Debug.LogWarning("No game objects in the list!");
            return;
        }
        
        SpawnCar();
        

    }
    
    private Vector3 GetCarTrailEndDirection()
    {
        Vector3 direction = carTrailEndPos.transform.position - transform.position;
        direction.Normalize();
        return direction;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position + new Vector3(0, 0, spawnOffsetZ);
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.2f);
        Gizmos.DrawWireCube(center, size);
    }

}
