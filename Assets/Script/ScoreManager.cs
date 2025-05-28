using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    // gameId -> (playerId -> score)
    private Dictionary<string, Dictionary<int, int>> bestScores = new();

    // Получение connectionId игрока
    private int GetPlayerId(NetworkConnectionToClient conn) => conn.connectionId;

    [Command(requiresAuthority = false)]
    public void CmdSubmitScore(string gameId, int score, NetworkConnectionToClient sender = null)
    {
        int playerId = GetPlayerId(sender);

        if (!bestScores.ContainsKey(gameId))
            bestScores[gameId] = new();

        var gameScores = bestScores[gameId];

        if (!gameScores.ContainsKey(playerId) || score > gameScores[playerId])
        {
            gameScores[playerId] = score;
            Debug.Log($"New record from Player {playerId} in {gameId}: {score}");
        }
    }

    // Запрос лучших 5 результатов для игры
    [Command(requiresAuthority = false)]
    public void CmdGetTopScores(string gameId, NetworkConnectionToClient sender = null)
    {
        List<ScoreEntry> topScores = new();

        if (bestScores.ContainsKey(gameId))
        {
            foreach (var kvp in bestScores[gameId])
                topScores.Add(new ScoreEntry { playerId = kvp.Key, score = kvp.Value });

            topScores.Sort((a, b) => b.score.CompareTo(a.score)); // По убыванию
            if (topScores.Count > 5) topScores = topScores.GetRange(0, 5);
        }

        TargetReceiveTopScores(sender, gameId, topScores.ToArray());
    }

    // Запрос всех рекордов для игрока
    [Command(requiresAuthority = false)]
    public void CmdGetPlayerScores(NetworkConnectionToClient sender = null)
    {
        int playerId = GetPlayerId(sender);
        List<ScoreEntry> personalScores = new();

        foreach (var game in bestScores)
        {
            if (game.Value.ContainsKey(playerId))
            {
                personalScores.Add(new ScoreEntry
                {
                    gameId = game.Key,
                    playerId = playerId,
                    score = game.Value[playerId]
                });
            }
        }

        TargetReceivePlayerScores(sender, personalScores.ToArray());
    }

    [TargetRpc]
    void TargetReceiveTopScores(NetworkConnection conn, string gameId, ScoreEntry[] scores)
    {
        ScoreUI.Instance?.ShowTopScores(gameId, scores);
    }

    [TargetRpc]
    void TargetReceivePlayerScores(NetworkConnection conn, ScoreEntry[] scores)
    {
        ScoreUI.Instance?.ShowPlayerScores(scores);
    }

    [System.Serializable]
    public struct ScoreEntry
    {
        public string gameId;
        public int playerId;
        public int score;
    }
}
