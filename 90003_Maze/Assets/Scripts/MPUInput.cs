using UnityEngine;
using System.IO.Ports;
 
public class MPUInput : MonoBehaviour
{
	[Header("Serial Settings")]
	public string portName = "COM4";
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
		if (sp == null || !sp.IsOpen) return;

		// ① 循环把缓冲区全部读完，只留下最后一行
		string latestLine = null;
		while (sp.BytesToRead > 0)
		{
			try      { latestLine = sp.ReadLine(); }
			catch    { break; }      // 超时也跳出
		}
		if (string.IsNullOrEmpty(latestLine)) return;

		ParseFrame(latestLine); 
	}
 
	void ParseFrame(string s)
	{
		s = s.Trim();
		if (!s.StartsWith("$") || !s.EndsWith("#")) return;

		int rStart = s.IndexOf('R');
		int yStart = s.IndexOf('Y');
		if (rStart < 0 || yStart < 0) return;

		int rollVal  = int.Parse(s.Substring(rStart+1, yStart-rStart-1));
		float rollDeg = rollVal / 100f;

		float horizontal = Mathf.Clamp(rollDeg / maxRollDeg, -1f, 1f);
		car.SetInput(horizontal, 0f);     // 忽略垂直
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
