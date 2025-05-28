using UnityEngine;

public class CameraTeleporter : MonoBehaviour
{
    [Header("Ссылка на камеру")]
    public Transform targetCamera;

    [Header("Координаты назначения (глобальные)")]
    public Vector3 targetPosition = new Vector3(0, 5, -10);

    [Header("Угол поворота (в градусах)")]
    public Vector3 targetRotationEuler = new Vector3(30, 0, 0);

    public void TeleportCamera()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("Камера не назначена!");
            return;
        }

        // Отключаем родителя, если есть
        targetCamera.SetParent(null);

        // Устанавливаем глобальную позицию
        targetCamera.position = targetPosition;

        // Устанавливаем глобальный угол поворота
        targetCamera.rotation = Quaternion.Euler(targetRotationEuler);

        Debug.Log("Камера перемещена в позицию: " + targetPosition + " с углом: " + targetRotationEuler);
    }
}