using UnityEngine;
using System.IO.Ports;

public class TripleSphereDetector : MonoBehaviour
{
    [Header("球形射线参数")]
    [Tooltip("球射线半径 (米)")]
    public float sphereRadius = 1.0f;

    [Tooltip("射线长度 (米)")]
    public float rayLength = 15f;

    [Tooltip("左右车道 X 偏移 (米)")]
    public float laneOffset = 3f;

    public LayerMask carLayer;              // 只检测来车层

    [Header("串口")]
    public string portName = "COM3";
    public int    baudRate = 9600;

    private SerialPort sp;

    // 记录上一帧状态，避免重复发送
    private byte lastL, lastF, lastR;

    // —— 供 Gizmo 使用的缓存 ——
    private Vector3 originL, originF, originR;
    private bool hitL, hitF, hitR;

    void Start()
    {
        sp = new SerialPort(portName, baudRate);
        sp.NewLine = "\n";
        sp.Open();
        sp.DtrEnable = true;
    }

    void Update()
    {
        // 计算三个射线起点
        Vector3 fwd   = transform.forward;
        originF       = transform.position;
        originL       = originF - transform.right * laneOffset;
        originR       = originF + transform.right * laneOffset;

        // ---------- SphereCast 三条并行射线 ----------
        hitF = Physics.SphereCast(originF, sphereRadius, fwd, out _, rayLength, carLayer);
        hitL = Physics.SphereCast(originL, sphereRadius, fwd, out _, rayLength, carLayer);
        hitR = Physics.SphereCast(originR, sphereRadius, fwd, out _, rayLength, carLayer);

        // Debug 线
        Debug.DrawRay(originL, fwd * rayLength, hitL ? Color.red : Color.green);
        Debug.DrawRay(originF, fwd * rayLength, hitF ? Color.red : Color.green);
        Debug.DrawRay(originR, fwd * rayLength, hitR ? Color.red : Color.green);

        // ---------- 串口通信 ----------
        byte L = (byte)(hitL ? 1 : 0);
        byte F = (byte)(hitF ? 1 : 0);
        byte R = (byte)(hitR ? 1 : 0);

        if (L != lastL || F != lastF || R != lastR)
        {
            SendFrame(L, F, R);
            lastL = L; lastF = F; lastR = R;
        }
    }

    // 在 Scene/Game 视图中绘制可视化球
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        DrawSphereGizmo(originL, hitL);
        DrawSphereGizmo(originF, hitF);
        DrawSphereGizmo(originR, hitR);
    }

    void DrawSphereGizmo(Vector3 origin, bool hit)
    {
        Gizmos.color = hit ? Color.red : Color.green;
        // 在起点向前偏移 radius，画一个球
        Gizmos.DrawWireSphere(origin + transform.forward * sphereRadius, sphereRadius);
    }

    // 封装 6 字节协议：AA L F R SUM 55
    void SendFrame(byte L, byte F, byte R)
    {
        byte sum = (byte)((L + F + R) & 0xFF);
        byte[] frame = { 0xAA, L, F, R, sum, 0x55 };
        if (sp != null && sp.IsOpen)
            sp.Write(frame, 0, frame.Length);
    }

    void OnApplicationQuit()
    {
        if (sp != null && sp.IsOpen) sp.Close();
    }
}
