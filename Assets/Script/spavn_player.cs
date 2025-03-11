using UnityEngine;
using Mirror;

public class SpavnPlayer : NetworkManager
{
    public override void OnServerSceneChanged(string sceneName)
    {
        foreach (NetworkConnection conn in NetworkServer.connections.Values)
        {
            NetworkConnectionToClient clientConn = conn as NetworkConnectionToClient;
            if (clientConn != null) // Проверяем, что приведение прошло успешно
            {
                GameObject player = Instantiate(playerPrefab, GetStartPosition().position, Quaternion.identity);
                NetworkServer.ReplacePlayerForConnection(clientConn, player);
            }
        }
    }
}
