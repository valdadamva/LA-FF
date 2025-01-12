using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera camera1;  // Ссылка на первую камеру
    public Camera camera2;  // Ссылка на вторую камеру
    public Vector3 targetPosition;  // Целевая позиция, при достижении которой происходит переключение
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
        if (Vector3.Distance(transform.position, targetPosition) < switchThreshold)
        {
            SwitchCameras();
        }
    }

    void SwitchCameras()
    {
        // Отключаем первую камеру и включаем вторую
        camera1.enabled = false;
        camera2.enabled = true;
    }
}