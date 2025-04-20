using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))] // Автоматически добавит компонент
[RequireComponent(typeof(Rigidbody))]     // Автоматически добавит компонент
public class InputManager : NetworkBehaviour
{
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    void Start()
    {
        // Получаем компоненты
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        // Проверяем наличие компонентов
        if (rb == null)
            Debug.LogError("Rigidbody component not found!", this);
        
        if (playerMovement == null)
            Debug.LogError("PlayerMovement component not found!", this);
    }

    void FixedUpdate()
    {
        // Проверяем что это локальный игрок и компоненты существуют
        if (!isLocalPlayer || rb == null || playerMovement == null)
            return;

        // Получаем ввод
        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        ).normalized;

        // Перемещаем персонажа
        MovePlayer(input.x, input.y);
    }

    void MovePlayer(float horizontal, float vertical)
    {
        // Рассчитываем движение
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * 
                           playerMovement.moveSpeed * 
                           Time.fixedDeltaTime; // Используем fixedDeltaTime для физики

        // Применяем движение
        rb.MovePosition(rb.position + movement);
    }
}