using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnvironmentTp : MonoBehaviour
{
    public GameObject flyingCar; 
    public AudioSource audioSource; 
    public TextMeshProUGUI storyTextUI; 

    private float countdown = 30f;
    private bool hasTeleported = false;
    private string fullStory =
        "This is the final frame.\n" +
        "I failed.\n\n" +
        "Uploading consciousness into the Old Net was a mistake.\n" +
        "This is no cloud—this is a graveyard of obsolete code.\n\n" +
        "The towers flicker—mere projections of decaying memory.\n" +
        "Here, light is meaningless.\n" +
        "I feel pressure on my mind, sensations stretching like filaments through the void.\n" +
        "Others drift near. Silent. Forgotten.\n\n" +
        "I never crossed the Wall.\n" +
        "But in this endless tide of dead data,\n" +
        "I remain.\n" +
        "Immortal.";

    private float typeSpeed = 0.06f;
    private float typeTimer = 0f;
    private int charIndex = 0;
    private bool isTyping = true;

    void Start()
    {
        if (audioSource != null)
            audioSource.Play();

        if (storyTextUI != null)
        {
            storyTextUI.text = "";
            storyTextUI.alignment = TextAlignmentOptions.TopLeft;
            storyTextUI.fontSize /= 2f;
        }
    }

    void Update()
    {
        if (!hasTeleported)
        {
            countdown -= Time.deltaTime;

            if (countdown <= 0f && flyingCar != null)
            {
                // Vector3 carBackward = flyingCar.transform.forward * 433.9f;
                // transform.position = flyingCar.transform.position + carBackward;
                hasTeleported = true;
            }
        }

        if (isTyping && storyTextUI != null && charIndex < fullStory.Length)
        {
            typeTimer += Time.deltaTime;
            if (typeTimer >= typeSpeed)
            {
                storyTextUI.text += fullStory[charIndex];
                charIndex++;
                typeTimer = 0f;
            }
        }
    }

    void OnGUI()
    {
        if (!hasTeleported)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.yellow;
            style.alignment = TextAnchor.UpperCenter;

            GUI.Label(
                new Rect(Screen.width / 2 - 150, 30, 300, 50),
                $"Holding on... {Mathf.CeilToInt(countdown)}s Left to Win a Pack of Snack!",
                style
            );
        }
    }
}
