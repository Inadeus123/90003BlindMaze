using UnityEngine;
using System.IO.Ports;

public class MPUInput : MonoBehaviour
{
    [Header("Serial")]
    public string portName = "COM4";      // 改成实际端口
    public int baudRate = 115200;

    [Header("Mapping")]
    public float maxPitchDeg = 20f;       // 前后倾 ±20° 对应满输入
    public float maxRollDeg  = 20f;       // 左右倾

    public FlyingCarController car;       // 引用飞车脚本

    SerialPort sp;

    void Start()
    {
        sp = new SerialPort(portName, baudRate);
        sp.NewLine = "\r\n";      // ←改成 CRLF
        sp.Open();
        sp.ReadTimeout = 50;
    }

    void Update()
    {
        if (sp != null && sp.IsOpen && sp.BytesToRead > 0)
        {
            try
            {
                string line = sp.ReadLine();   // 拿到一行完整帧
                ParseFrame(line);
            }
            catch (System.TimeoutException) { }
        }
    }

    void ParseFrame(string s)
    {
        // 格式: $P1234R-0567Y0012#
        s = s.Trim();    // 防止意外空白
        if (!s.StartsWith("$") || !s.EndsWith("#"))
        {
            //Debug.Log("111111111");

            return;
        }

        ;
        int pStart = s.IndexOf('P');
        int rStart = s.IndexOf('R');
        int yStart = s.IndexOf('Y');
        if (pStart < 0 || rStart < 0 || yStart < 0) {
            //Debug.Log("2222222222");

            return;
        };

        int pitchVal = int.Parse(s.Substring(pStart+1, rStart-pStart-1));
        int rollVal  = int.Parse(s.Substring(rStart+1, yStart-rStart-1));
        
        // yawVal 可选
        // int yawVal   = int.Parse(s.Substring(yStart+1, s.Length-yStart-2));

        float pitchDeg = pitchVal / 100f;
        float rollDeg  = rollVal  / 100f;

        // 归一化成 [-1,1] 输入
        //float verticalInput  = Mathf.Clamp((pitchDeg / maxPitchDeg)*10, -1f, 1f);
        float horizontalInput= Mathf.Clamp((rollDeg  / maxRollDeg) *10, -1f, 1f);

        //Debug.Log("verticalInput"+verticalInput);
        Debug.Log("horizontalInput"+horizontalInput);
        // 丢给飞车控制器
        car.SetInput(horizontalInput, 0);
    }

    void OnApplicationQuit()
    {
        if (sp != null && sp.IsOpen) sp.Close();
    }
}