using UnityEngine;
using Mirror;

public class minigameCam : NetworkBehaviour
{
    public Camera playerCamera;

    public void StartMiniGame()
    {
        if (!isLocalPlayer) return;

        if (playerCamera != null)
            playerCamera.enabled = false;

      
    }

    public void StopMiniGame()
    {
        if (!isLocalPlayer) return;

        if (playerCamera != null)
            playerCamera.enabled = true;
    }
}
