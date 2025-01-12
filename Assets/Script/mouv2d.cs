using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f;           // Сила прыжка
    public float moveSpeed = 5f;           // Скорость движения
    public Transform groundCheck;          // Позиция для проверки земли
    public LayerMask groundLayer;          // Слой земли
    public float groundCheckRadius = 0.2f; // Радиус проверки земли

    private Rigidbody2D rb;
    private bool isGrounded;               // Флаг, показывающий, находимся ли мы на земле

    void Start()
    {
        // Получаем компонент Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Проверка, находимся ли мы на земле
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Движение персонажа влево/вправ
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);  // Поддерживаем вертикальную скорость (Y) при движении по X

        // Прыжок, если на земле
        if (isGrounded && Input.GetButtonDown("Jump"))  // Используем кнопку "Jump" (например, пробел)
        {
            Jump();
        }
    }

    void Jump()
    {
        // Применяем силу прыжка по оси Y, оставляя текущую скорость по оси X
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // Отображение радиуса проверки земли (для отладки)
    private void OnDrawGizmos()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);  // Показываем радиус проверки земли
    }
}



