using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTurnTrigger : MonoBehaviour
{
    public Vector3 moveDir = Vector3.forward;
    public float speed = 10f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turn"))
        {
            // 拐角触发：读取方向点提供的新方向
            Vector3 newDir = other.transform.forward;
            moveDir = newDir;

            // 同步旋转（可选）
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }
}
