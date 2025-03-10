using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror; // Добавляем Mirror

public class SceneTriggerMirror : NetworkBehaviour
{
    public string sceneToLoad; // Название сцены для загрузки

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Проверяем, что в зону вошёл игрок
        {
            NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();

            if (networkIdentity != null && networkIdentity.isLocalPlayer) // Только локальный игрок
            {
                LoadSceneForLocalPlayer();
            }
        }
    }

    private void LoadSceneForLocalPlayer()
    {
        SceneManager.LoadScene(sceneToLoad); // Загружаем сцену только для одного игрока
    }
}