using UnityEngine;
using System.IO.Ports;

public class MPUInput : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/tty.usbserial-564E0546631";
    public int baudRate = 115200;

    [Header("Mapping")]
    public float maxPitchDeg = 90f;
    public float maxRollDeg = 90f;

    public FlyingCarController car; 

    SerialPort sp;

    void Start()
    {
        try
        {
            sp = new SerialPort(portName, baudRate);
            sp.NewLine = "\r\n";
            sp.Open();
            sp.ReadTimeout = 50;
            Debug.Log($"[MPUInput] Serial port {portName} opened.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[MPUInput] Failed to open serial port: {e.Message}");
        }
    }

    void Update()
    {
        if (sp != null && sp.IsOpen && sp.BytesToRead > 0)
        {
            try
            {
                string line = sp.ReadLine();
                Debug.Log("[MPUInput] Raw line: " + line);
                ParseFrame(line);
            }
            catch (System.TimeoutException) { }
        }
    }

    void ParseFrame(string s)
    {
        s = s.Trim();
        if (!s.StartsWith("$") || !s.EndsWith("#"))
        {
            Debug.LogWarning("[MPUInput] Invalid frame format: " + s);
            return;
        }

        int pStart = s.IndexOf('P');
        int rStart = s.IndexOf('R');
        int yStart = s.IndexOf('Y');
        if (pStart < 0 || rStart < 0 || yStart < 0)
        {
            Debug.LogWarning("[MPUInput] Missing P/R/Y markers in: " + s);
            return;
        }

        try
        {
            int pitchVal = int.Parse(s.Substring(pStart + 1, rStart - pStart - 1));
            int rollVal = int.Parse(s.Substring(rStart + 1, yStart - rStart - 1));

            float pitchDeg = pitchVal / 100f;
            float rollDeg = rollVal / 100f;

            float horizontalInput = Mathf.Clamp(rollDeg / maxRollDeg, -1f, 1f);
            float verticalInput = Mathf.Clamp(pitchDeg / maxPitchDeg, -1f, 1f);

            Debug.Log($"[MPUInput] Parsed Pitch: {pitchDeg}°, Roll: {rollDeg}° => Input (H={horizontalInput}, V={verticalInput})");

            if (car != null)
                car.SetInput(horizontalInput, verticalInput);
            else
                Debug.LogWarning("[MPUInput] Car reference not assigned!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[MPUInput] Parse error: " + ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (sp != null && sp.IsOpen)
        {
            sp.Close();
            Debug.Log("[MPUInput] Serial port closed.");
        }
    }
}
