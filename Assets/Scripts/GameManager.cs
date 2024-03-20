using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float gameTime = 20f;
    public TextMeshPro timerText;
    public TextMeshPro scoreText;
    public GameObject ball;
    public GameObject[] penguins;

    private int scoreRedTeam;
    private int scoreBlueTeam;
    private float currentTime;

    private void Start()
    {
        currentTime = gameTime;
        UpdateTimerText();
        UpdateScoreText();
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        UpdateTimerText();

        if (currentTime <= 0f)
        {
            EndGame("Time's up!");
        }
    }

    public void GoalScored(int teamIndex)
    {
        if (teamIndex == 1)
        {
            scoreRedTeam++;
        }
        else if (teamIndex == 2)
        {
            scoreBlueTeam++;
        }

        UpdateScoreText();
        
        // Immediately check if scoring the goal ends the game
        if (scoreRedTeam >= 3)
        {
            EndGame("Red team wins!");
        }
        else if (scoreBlueTeam >= 3)
        {
            EndGame("Blue team wins!");
        }

        ball.GetComponent<BallController>().Respawn();
        foreach (GameObject penguin in penguins)
        {
            penguin.GetComponent<PenguinController>().Respawn();
        }
    }

    private void EndGame(string message)
    {
        Debug.Log(message);
        Application.Quit();

        // Quit the editor play mode if running in the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateScoreText()
    {
        scoreText.text = string.Format("{0}-{1}", scoreRedTeam, scoreBlueTeam);
    }
}
