using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    public GameObject panel;
    public TMP_Text topScoresText;
    public TMP_Text playerScoresText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
        }
    }


    public void ShowTopScores(string gameId, ScoreManager.ScoreEntry[] scores)
    {
        topScoresText.text = $"🏆 Top scores for {gameId}:\n";
        foreach (var score in scores)
        {
            topScoresText.text += $"Player {score.playerId}: {score.score}\n";
        }
    }

    public void ShowPlayerScores(ScoreManager.ScoreEntry[] scores)
    {
        playerScoresText.text = $"👤 Your Records:\n";
        foreach (var score in scores)
        {
            playerScoresText.text += $"{score.gameId}: {score.score}\n";
        }
    }
}