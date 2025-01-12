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
 // Этот метод вызывается только для локального игрока
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Убедимся, что камера активируется только для локального игрока
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true); // Включаем камеру
        }
    }
}


