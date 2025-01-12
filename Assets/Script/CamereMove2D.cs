using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera camera1;  // Ссылка на первую камеру
    public Camera camera2;  // Ссылка на вторую камеру
    public float switchThreshold = 1f; // Точность для достижения целевой позиции (погрешность)

    void Start()
    {
        // Изначально камера1 активна, а камера2 отключена
        camera1.enabled = true;
        camera2.enabled = false;
    }

    void Update()
    {
        // Проверяем, достиг ли игрок целевой позиции
        if (transform.position.x > (float)11)
        {
            SwitchCameras();
        }
		else
		{
			Start();
		}
 
    }

    void SwitchCameras()
    {
        // Отключаем первую камеру и включаем вторую
        camera1.enabled = false;
        camera2.enabled = true;
    }
}
