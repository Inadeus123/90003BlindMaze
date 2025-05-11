using UnityEngine;
using System.Collections.Generic;

public class CitySpawner : MonoBehaviour
{
    [Header("引用设置")]
    public Transform player;             // 飞车（玩家）Transform，用于获取位置
    public GameObject[] buildingPrefabs; // 建筑物预制体数组，从中随机选择生成

    [Header("城市生成参数")]
    public float distanceBetween = 50f;    // 每段建筑之间的间隔距离（沿前进方向Z轴）
    public int initialSegments = 10;       // 初始生成的片段数（每片段包含左右各一栋楼）
    public int maxSegments = 12;           // 保持的最大片段数，超过则删除最旧的一段

    public float laneHalfWidth = 30f;      // 飞行通道的一半宽度，即左右建筑离中心的距离

    private float spawnZPosition = 0f;     // 下一次生成建筑的Z坐标位置
    private Queue<GameObject> spawnedBuildings = new Queue<GameObject>(); // 存储当前激活的建筑对象队列

    void Start()
    {
        // 初始化生成初始建筑片段
        spawnZPosition = player.position.z;
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnNextSegment();
        }
    }

    void Update()
    {
        // 如果玩家向前推进到接近最后生成的位置，则继续生成新的片段
        float playerZ = player.position.z;
        // 条件：玩家距离最新生成点不足 (distanceBetween * 段数) 时，提前生成下一个
        if (playerZ + (distanceBetween * (initialSegments - 2)) > spawnZPosition)
        {
            // 生成新片段（左右各一栋楼）
            SpawnNextSegment();

            // 如果当前片段总数超过最大保留数，则清理最旧的片段
            if (spawnedBuildings.Count > maxSegments * 2) // 每段2栋楼，所以乘2
            {
                // 移除最早的一对建筑
                GameObject oldBuilding = spawnedBuildings.Dequeue();
                Destroy(oldBuilding);
                oldBuilding = spawnedBuildings.Dequeue();
                Destroy(oldBuilding);
            }
        }
    }

    void SpawnNextSegment()
    {
        // 左侧建筑的位置：在飞车左侧固定偏移 laneHalfWidth
        Vector3 leftPos = new Vector3(-laneHalfWidth, 0f, spawnZPosition);
        // 右侧建筑的位置：在飞车右侧固定偏移 laneHalfWidth
        Vector3 rightPos = new Vector3(laneHalfWidth, 0f, spawnZPosition);

        // 从预制体列表随机选择一种建筑
        GameObject leftPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
        GameObject rightPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];

        // 实例化左侧建筑
        GameObject leftBuilding = Instantiate(leftPrefab, leftPos, Quaternion.identity);
        // 实例化右侧建筑
        GameObject rightBuilding = Instantiate(rightPrefab, rightPos, Quaternion.identity);

        // 可选：对生成的建筑进行随机旋转朝向，使城市更不规则
        leftBuilding.transform.Rotate(0f, Random.Range(0, 4) * 90f, 0f);
        rightBuilding.transform.Rotate(0f, Random.Range(0, 4) * 90f, 0f);

        // 将新建的建筑加入队列尾部
        spawnedBuildings.Enqueue(leftBuilding);
        spawnedBuildings.Enqueue(rightBuilding);

        // 更新下次生成的位置，沿Z轴前移
        spawnZPosition += distanceBetween;
    }
}
