 using System;
 using GGG.Tool.Singleton;
 using UnityEngine;

public class DebugInfoDisplay : Singleton<DebugInfoDisplay>
{
    public float PlayerMovementInput;
    public bool IsPlayerRunning;
    public bool HasInput;
    public float PlayerRotationAngle;
    private GUIStyle guiStyle;

    protected override void Awake()
    {
        base.Awake();
        guiStyle = new GUIStyle
        {
            fontSize = 50,
            normal = { textColor = Color.green }
        };
    }

    private void OnGUI()
    {
        string debugText = "";
        //debugText += $"Is Player Running: {IsPlayerRunning}\n";
        debugText += $"Has Input: {HasInput}\n";
        debugText += $"Player Movement Input: {PlayerMovementInput}\n";
        debugText += $"Player Rotation Angle: {PlayerRotationAngle}\n";
        
        
        GUI.Label(new Rect(0, 0, 500, 200), debugText, guiStyle);   
    }
}
