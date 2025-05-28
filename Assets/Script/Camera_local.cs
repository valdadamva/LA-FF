using UnityEngine;
using Mirror;

public class PlayerCameraController : NetworkBehaviour
{
    public GameObject playerCamera; // Ссылка на камеру игрока

    
    
    
    void Awake()
    {
        if (playerCamera == null)
            playerCamera = transform.Find("Cameraaaaaaa").gameObject; // Заменить "CameraName" на реальное имя камеры
    }
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
    
    public void StartMiniGame()
    {
        if (isLocalPlayer)
        {
            if (playerCamera != null)
                playerCamera.SetActive(false);
        }

    }

    public void StopMiniGame()
    {
        if (isLocalPlayer)
        {
            if (playerCamera != null)
                playerCamera.SetActive(true);
        }
    }
}


