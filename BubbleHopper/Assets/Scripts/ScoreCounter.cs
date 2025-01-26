using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int scoreIncrement = 10;
    [SerializeField] private AudioClip pop01;

    private int score = 000;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        UpdateScoreUI();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bubble"))
        {
            score += scoreIncrement;
            UpdateScoreUI();

            if (pop01 != null && audioSource != null)
            {
                audioSource.PlayOneShot(pop01);
            }

            Destroy(collision.gameObject);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            // integer to text
            scoreText.text = score.ToString("D3");
        }
    }
}