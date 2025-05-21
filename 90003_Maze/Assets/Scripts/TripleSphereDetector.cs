using UnityEngine;

public class TripleSphereDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float radius = 1f;
    public float rayLen = 15f;
    public float laneOffset = 3f;
    public LayerMask carLayer;

    [Header("Alarm UI & Audio")]
    public AudioSource alarmAudio;
    public float flashInterval = 0.2f; // Flashing every 0.2s

    private bool isAlarmActive = false;
    private float flashTimer = 0f;
    private bool flashOn = false;

    void Update()
    {
        Vector3 fwd = transform.forward;
        Vector3 originF = transform.position;
        Vector3 originL = originF - transform.right * laneOffset;
        Vector3 originR = originF + transform.right * laneOffset;

        bool hitL = Physics.SphereCast(originL, radius, fwd, out _, rayLen, carLayer);
        bool hitF = Physics.SphereCast(originF, radius, fwd, out _, rayLen, carLayer);
        bool hitR = Physics.SphereCast(originR, radius, fwd, out _, rayLen, carLayer);

        bool detected = hitL || hitF || hitR;

        if (detected)
        {
            if (!isAlarmActive)
                TriggerAlarm();
        }
        else
        {
            if (isAlarmActive)
                StopAlarm();
        }

        if (isAlarmActive)
        {
            flashTimer += Time.deltaTime;
            if (flashTimer >= flashInterval)
            {
                flashOn = !flashOn;
                flashTimer = 0f;
            }
        }

        // Debug rays
        Debug.DrawRay(originL, fwd * rayLen, hitL ? Color.red : Color.green);
        Debug.DrawRay(originF, fwd * rayLen, hitF ? Color.red : Color.green);
        Debug.DrawRay(originR, fwd * rayLen, hitR ? Color.red : Color.green);
    }

    void TriggerAlarm()
    {
        isAlarmActive = true;
        flashOn = true;
        flashTimer = 0f;

        if (alarmAudio != null && !alarmAudio.isPlaying)
            alarmAudio.Play();
    }

    void StopAlarm()
    {
        isAlarmActive = false;
        flashOn = false;
        flashTimer = 0f;

        if (alarmAudio != null && alarmAudio.isPlaying)
            alarmAudio.Stop();
    }

    void OnGUI()
    {
        if (isAlarmActive && flashOn)
        {
            Color redOverlay = new Color(1f, 0f, 0f, 0.35f);
            GUI.color = redOverlay;

            float thickness = 40f;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, thickness), Texture2D.whiteTexture); // Top
            GUI.DrawTexture(new Rect(0, Screen.height - thickness, Screen.width, thickness), Texture2D.whiteTexture); // Bottom
            GUI.DrawTexture(new Rect(0, 0, thickness, Screen.height), Texture2D.whiteTexture); // Left
            GUI.DrawTexture(new Rect(Screen.width - thickness, 0, thickness, Screen.height), Texture2D.whiteTexture); // Right
        }
    }

    void OnDrawGizmos()
    {
        // Draw wire spheres in Scene View for detection areas
        Vector3 fwd = transform.forward;
        Vector3 originF = transform.position;
        Vector3 originL = originF - transform.right * laneOffset;
        Vector3 originR = originF + transform.right * laneOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(originL + fwd * rayLen, radius);
        Gizmos.DrawWireSphere(originF + fwd * rayLen, radius);
        Gizmos.DrawWireSphere(originR + fwd * rayLen, radius);

    }
}
