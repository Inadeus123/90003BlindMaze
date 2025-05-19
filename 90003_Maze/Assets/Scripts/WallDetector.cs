using UnityEngine;
using System.IO.Ports;

public class WallDetector : MonoBehaviour
{
    [Header("Raycast")]
    public float maxDistance = 0.8f;            // 距墙判定阈值
    public LayerMask wallMask;                  // 设置为 "Wall"
    
    [Header("Serial")]
    public string portName = "COM3";            // 改成你的端口
    public int baudRate = 9600;
    [Tooltip("射线检测最大距离")]
    public float maxDetectDistance = 3.0f;
    private SerialPort sp;
    private bool isInflating = false;           // 当前状态，防止指令狂发

    void Start()
    {
        if (sp == null)
        {
            sp = new SerialPort(portName, baudRate);

        }

        if (!sp.IsOpen)
        {
            
            sp.Open();
        }
        sp.DtrEnable = true;                    // 部分USB转串需要
    }

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 forwardDir = transform.forward;
        float distFront = maxDetectDistance;
        
        bool hitWall = Physics.Raycast(
            origin,
            forwardDir,
            hitInfo: out RaycastHit hit,
            maxDistance,
            wallMask);

        if (hitWall && !isInflating)
        {
            Debug.Log("Hit Car: " + hit.distance);
            SendCommand('I');                   // 命令充气
            isInflating = true;
        }
        else if (!hitWall && isInflating)
        {
            SendCommand('S');                   // 命令停气
            isInflating = false;
        }
        
        Debug.DrawRay(origin, forwardDir * distFront, Color.red);
    }

    void SendCommand(char c)
    {
        if (sp != null && sp.IsOpen)
        {
            sp.Write(new char[] { c }, 0, 1);   // 发送单字符
            Debug.Log("SendCommand:" + c);
        }
    }

    void OnApplicationQuit()
    {
        if (sp != null && sp.IsOpen) sp.Close();
    }
}