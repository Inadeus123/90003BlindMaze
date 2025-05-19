using UnityEngine;
using System.IO.Ports;

public class TripleSphereDetector : MonoBehaviour
{
    public float radius = 1f;
    public float rayLen = 15f;
    public float laneOffset = 3f;
    public LayerMask carLayer;

    SerialPort sp;
    public string port = "COM3";
    void Start(){
        sp = new SerialPort(port,9600);
        sp.Open();
    }

    // 上一帧是否命中
    bool lastHitL, lastHitF, lastHitR;

    void Update(){
        Vector3 fwd = transform.forward;
        Vector3 originF = transform.position;
        Vector3 originL = originF - transform.right * laneOffset;
        Vector3 originR = originF + transform.right * laneOffset;

        bool hitL = Physics.SphereCast(originL,radius,fwd,out _,rayLen,carLayer);
        bool hitF = Physics.SphereCast(originF,radius,fwd,out _,rayLen,carLayer);
        bool hitR = Physics.SphereCast(originR,radius,fwd,out _,rayLen,carLayer);

        Debug.DrawRay(originL, fwd*rayLen, hitL?Color.red:Color.green);
        Debug.DrawRay(originF, fwd*rayLen, hitF?Color.red:Color.green);
        Debug.DrawRay(originR, fwd*rayLen, hitR?Color.red:Color.green);

        // 左通道
        if(hitL!=lastHitL){
            sp.Write(hitL ? "A" : "a");  // A=充气, a=放气
            lastHitL = hitL;
        }
        // 中通道
        if(hitF!=lastHitF){
            sp.Write(hitF ? "B" : "b");
            lastHitF = hitF;
        }
        // 右通道
        if(hitR!=lastHitR){
            sp.Write(hitR ? "C" : "c");
            lastHitR = hitR;
        }
    }

    void OnApplicationQuit(){ if(sp!=null&&sp.IsOpen) sp.Close(); }
}