using UnityEngine;
using UnityEngine.SceneManagement;  // Не забудьте подключить эту библиотеку

public class SceneLoader : MonoBehaviour
{
    // Метод для загрузки сцены по имени
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}