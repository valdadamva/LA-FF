using UnityEngine; // Добавьте эту строку, если её нет

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Теперь компилятор сможет найти Transform

    void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }
}
