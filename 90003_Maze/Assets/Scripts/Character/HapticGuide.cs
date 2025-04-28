using UnityEngine;
using System.IO.Ports;

public class HapticGuide : MonoBehaviour
{
    [Tooltip("串口端口名称")]
    public string portName = "COM3";
    [Tooltip("串口波特率")]
    public int baudRate = 115200;
    [Tooltip("墙壁所在层的名称")]
    public string wallLayerName = "Wall";
    [Tooltip("射线检测最大距离")]
    public float maxDetectDistance = 3.0f;
    [Tooltip("左前方射线相对前方的水平偏角")]
    public float leftAngle = -45f;
    [Tooltip("右前方射线相对前方的水平偏角")]
    public float rightAngle = 45f;
    
    private SerialPort serial;
    private LayerMask wallMask;
    
    void Start()
    {
        // 获取墙壁层Mask
        wallMask = LayerMask.GetMask(wallLayerName);
        // 初始化并打开串口
        serial = new SerialPort(portName, baudRate);
        serial.Open();
        serial.DtrEnable = false;
    }
    
    void Update()
    {
        // 定义射线方向
        Vector3 origin = transform.position;
        Vector3 forwardDir = transform.forward;
        Vector3 leftForwardDir = Quaternion.Euler(0, leftAngle, 0) * forwardDir;
        Vector3 rightForwardDir = Quaternion.Euler(0, rightAngle, 0) * forwardDir;
        
        // 发射三束射线并获取距离
        float distFront = maxDetectDistance;
        float distLeft = maxDetectDistance;
        float distRight = maxDetectDistance;
        RaycastHit hit;
        if (Physics.Raycast(origin, forwardDir, out hit, maxDetectDistance, wallMask))
            distFront = hit.distance;
        if (Physics.Raycast(origin, leftForwardDir, out hit, maxDetectDistance, wallMask))
            distLeft = hit.distance;
        if (Physics.Raycast(origin, rightForwardDir, out hit, maxDetectDistance, wallMask))
            distRight = hit.distance;
        
        // 映射距离到压力值
        int pressureFront = (distFront >= maxDetectDistance) ? 0 :
                            Mathf.Clamp((int)((maxDetectDistance - distFront) / maxDetectDistance * 255), 0, 255);
        int pressureLeft  = (distLeft  >= maxDetectDistance) ? 0 :
                            Mathf.Clamp((int)((maxDetectDistance - distLeft)  / maxDetectDistance * 255), 0, 255);
        int pressureRight = (distRight >= maxDetectDistance) ? 0 :
                            Mathf.Clamp((int)((maxDetectDistance - distRight) / maxDetectDistance * 255), 0, 255);
        
        byte[] frame = new byte[6];
        frame[0] = 0xAA;
        frame[1] = (byte)pressureLeft;
        frame[2] = (byte)pressureFront;
        frame[3] = (byte)pressureRight;
        int sum = pressureLeft + pressureFront + pressureRight;
        frame[4] = (byte)(sum & 0xFF);
        frame[5] = 0x55;
        if (serial.IsOpen) {
            serial.Write(frame, 0, 6);
        }
        
        //调试
        Debug.DrawRay(origin, leftForwardDir * distLeft, Color.red);
        Debug.DrawRay(origin, forwardDir * distFront, Color.red);
        Debug.DrawRay(origin, rightForwardDir * distRight, Color.red);
        Debug.Log($"L:{pressureLeft} F:{pressureFront} R:{pressureRight}");
    }
    
    void OnApplicationQuit()
    {
        // 退出时关闭串口
        if (serial != null && serial.IsOpen) {
            serial.Close();
        }
    }
}
