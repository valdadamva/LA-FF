using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float smoothing = 1.5f;
    [SerializeField] private Transform character;
    [SerializeField] private bool invertY = false;

    [Header("Cursor Settings")]
    [SerializeField] private bool lockCursorOnStart = true;

    private Vector2 velocity;
    private Vector2 frameVelocity;
    private bool cameraEnabled = true;
    

    void Reset()
    {
        // Автопоиск трансформа персонажа, если не задан
        character = GetComponentInParent<PlayerMovement>()?.transform;
    }

    void Start()
    {
        if (lockCursorOnStart)
        {
            SetCursorState(false); // Блокировка курсора при старте
        }
    }

    void Update()
    {
        if (!cameraEnabled) return;

        // Получение ввода мыши
        Vector2 mouseDelta = new Vector2(
            Input.GetAxisRaw("Mouse X") * sensitivity,
            Input.GetAxisRaw("Mouse Y") * sensitivity * (invertY ? -1 : 1)
        );

        // Сглаживание
        frameVelocity = Vector2.Lerp(frameVelocity, mouseDelta, 1f / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90f, 90f);

        // Применение вращения
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }

    /// <summary> Включение/выключение управления камерой </summary>
    public void SetCameraEnabled(bool enabled)
    {
        cameraEnabled = enabled;

        if (!enabled)
        {
            frameVelocity = Vector2.zero;
            velocity = Vector2.zero;
        }
    }


    public void SetCursorState(bool showCursor)
    {
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showCursor;
    }

    // Для дебага (опционально)
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !Cursor.visible)
        {
            SetCursorState(false);
        }
    }
}