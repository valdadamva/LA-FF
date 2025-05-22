using Mirror;

public class ScoreClient : NetworkBehaviour
{
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    public void ShowScores(string gameId)
    {
        if (!isLocalPlayer) return;

        scoreManager.CmdGetTopScores(gameId);
        scoreManager.CmdGetPlayerScores();
        ScoreUI.Instance.TogglePanel();
    }

    public void SendScore(string gameId, int score)
    {
        if (!isLocalPlayer) return;

        scoreManager.CmdSubmitScore(gameId, score);
    }
}