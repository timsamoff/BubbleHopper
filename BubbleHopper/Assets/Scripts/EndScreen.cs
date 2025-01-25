using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private Text latestTimeText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text resultMessageText;

    private void Start()
    {
        // Retrieve latest and highest recorded times from PlayerPrefs
        string latestTime = PlayerPrefs.GetString("LatestGameTime", "00:00:00");
        string highScore = PlayerPrefs.GetString("HighScoreTime", "00:00:00");

        // Display latest and highest times
        latestTimeText.text = "Latest Time: " + latestTime;
        highScoreText.text = "Best Time: " + highScore;

        // Determine result message
        int comparisonResult = CompareTimes(latestTime, highScore);

        if (comparisonResult > 0)
        {
            PlayerPrefs.SetString("HighScoreTime", latestTime);  // New high score
            PlayerPrefs.Save();
            highScoreText.text = "Best Time: " + latestTime;  // Update high score text
            resultMessageText.text = "You won! New high score!";
        }
        else if (comparisonResult < 0)
        {
            resultMessageText.text = "Better luck next time!";
        }
        else
        {
            resultMessageText.text = "It's a tie!";
        }
    }

    private int CompareTimes(string time1, string time2)
    {
        float timeValue1 = ConvertTimeToFloat(time1);
        float timeValue2 = ConvertTimeToFloat(time2);

        if (timeValue1 > timeValue2) return 1;  // Latest time is higher (player won)
        if (timeValue1 < timeValue2) return -1; // High score is still greater (player lost)
        return 0;                               // Times are equal (tie)
    }

    private float ConvertTimeToFloat(string time)
    {
        string[] parts = time.Split(':');
        int minutes = int.Parse(parts[0]);
        int seconds = int.Parse(parts[1]);
        int milliseconds = int.Parse(parts[2]);

        return minutes * 60f + seconds + milliseconds / 100f;
    }
}