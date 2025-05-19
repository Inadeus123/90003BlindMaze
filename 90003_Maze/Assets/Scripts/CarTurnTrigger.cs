using System.Collections;
using UnityEngine;

public class CarTurnTrigger : MonoBehaviour
{
    [Header("转弯设定")]
    public float turnAngle    = 90f;   // +右转  -左转
    public float turnDuration = 0.4f;  // 秒

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        FlyingCarController ctrl = other.GetComponent<FlyingCarController>();
        if (ctrl == null) return;

        ctrl.inputLocked = true;                         // 锁输入
        //StartCoroutine(SmoothTurn(other.transform, ctrl));
        other.transform.rotation = Quaternion.Euler(0f, turnAngle, 0f);
    }

    IEnumerator SmoothTurn(Transform target, FlyingCarController ctrl)
    {
        Quaternion startRot = target.rotation;
        Quaternion endRot   = startRot * Quaternion.Euler(0, turnAngle, 0);

        float t = 0f;
        while (t < turnDuration)
        {
            t += Time.deltaTime;
            float lerp = t / turnDuration;
            target.rotation = Quaternion.Slerp(startRot, endRot, lerp); 
            yield return null;                                         
        }
        target.rotation = endRot;       
        //ctrl.inputLocked = false;      
    }
}