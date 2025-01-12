using Mirror;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    public Camera playerCamera;

    void Start()
    {
        // Если это локальный игрок, активируем камеру
        if (isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(true); // Включаем камеру
        }
        else
        {
            playerCamera.gameObject.SetActive(false); // Отключаем камеру для других игроков
        }
    }
}