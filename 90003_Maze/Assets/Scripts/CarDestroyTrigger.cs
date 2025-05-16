using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDestroyTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            Debug.Log("Car destroyed");
            Destroy(other.gameObject);
        }
    }
}
