using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance;

    // In-game time stored in seconds
    public float inGameTime;
    // Define a time multiplier if you want in-game time to pass faster or slower than real time.
    public float timeMultiplier = 1f;

    public TimeHandler timeHandler;

    [Tooltip("Define the start time. E.g.: 11:00am in seconds since midnight (11 * 3600 = 39600)")]
    // 3600 seconds in an hour
    public float startTimeInSeconds = 8f * 3600;

    public TextMeshProUGUI timeText;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Initialize time to the starting point
            inGameTime = startTimeInSeconds;

            timeHandler = new TimeHandler();
            timeHandler.UpdateTime(inGameTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Increment the in-game time. 
        // Adjust timeMultiplier as needed (for instance, 2f for double speed).
        inGameTime += Time.deltaTime * timeMultiplier;

        // Convert the in-game time from seconds to hours, minutes, and seconds
        int hours = Mathf.FloorToInt(inGameTime / 3600) % 24;
        int minutes = Mathf.FloorToInt((inGameTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(inGameTime % 60);

        //Debug.Log(string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds));

        if (timeText != null)
        {
            timeText.text = string.Format("{0:D2}:{1:D2}", timeHandler.Hour, timeHandler.Minute);
        }
    }
}
