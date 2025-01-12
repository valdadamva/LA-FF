using Mirror;
using UnityEngine;


public class InputManager : NetworkBehaviour
{
    private Rigidbody rb;
    private PlayerMovement playerMovement;  // Объявляем переменную для ссылки на PlayerMovement

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Получаем ссылку на PlayerMovement
        playerMovement = GetComponent<PlayerMovement>();  // Нужно убедиться, что компонент PlayerMovement есть на этом объекте
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on this GameObject.");
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component is null.");
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Используем переменную moveSpeed из PlayerMovement
        MovePlayer(horizontal, vertical);
    }

    void MovePlayer(float horizontal, float vertical)
    {
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized * playerMovement.moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }
}
