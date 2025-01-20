using UnityEngine;
using Mirror;

public class PlayerCameraController : NetworkBehaviour
{
    public GameObject playerCamera; // Ссылка на камеру игрока

    void Start()
    {
        if (isLocalPlayer)
        {
            playerCamera.SetActive(true); // Активируем камеру для локального игрока
        }
        else
        {
            playerCamera.SetActive(false); // Отключаем камеру для остальных игроков
        }
    }
}


