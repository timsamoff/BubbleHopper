using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private string waterTag = "Water";

    private float elapsedTime = 0f;
    private bool isRunning = true;
    private string lastRecordedTime = "00:00:00";

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(waterTag))
        {
            StopTimer();
        }
    }

    private void StopTimer()
    {
        isRunning = false;
        SaveTimeRecords();
    }

    private void SaveTimeRecords()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

        lastRecordedTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

        // Save latest game time
        PlayerPrefs.SetString("LatestGameTime", lastRecordedTime);

        // Save highest recorded time if greater than the previous best
        if (!PlayerPrefs.HasKey("HighScoreTime") || CompareTimes(lastRecordedTime, PlayerPrefs.GetString("HighScoreTime")))
        {
            PlayerPrefs.SetString("HighScoreTime", lastRecordedTime);
        }

        PlayerPrefs.Save();

        Debug.Log("Latest Time Saved: " + lastRecordedTime);
        Debug.Log("Best Time Saved: " + PlayerPrefs.GetString("HighScoreTime"));
    }

    private bool CompareTimes(string newTime, string bestTime)
    {
        // Convert time strings to float for comparison (mm:ss:ms)
        float newTimeValue = ConvertTimeToFloat(newTime);
        float bestTimeValue = ConvertTimeToFloat(bestTime);

        return newTimeValue > bestTimeValue;  // Higher time means longer survival
    }

    private float ConvertTimeToFloat(string time)
    {
        string[] parts = time.Split(':');
        int minutes = int.Parse(parts[0]);
        int seconds = int.Parse(parts[1]);
        int milliseconds = int.Parse(parts[2]);

        return minutes * 60f + seconds + milliseconds / 100f;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
        timerText.text = "00:00:00";
    }
}