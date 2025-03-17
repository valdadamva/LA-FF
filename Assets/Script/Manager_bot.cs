using UnityEngine;

public class BotMover : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость передвижения
    private Transform currentTarget; // Текущая цель
    private NavigationManager navigationManager; // Ссылка на менеджер навигации

    void Start()
    {
        // Находим менеджер навигации
        navigationManager = FindObjectOfType<NavigationManager>();

        if (navigationManager == null)
        {
            Debug.LogError("NavigationManager не найден!");
            enabled = false;
            return;
        }

        // Выбираем первую цель
        SetNewTarget();
    }    

    void Update()
    {
        if (currentTarget == null) return;

        // Перемещаемся к цели
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        // Проверяем, достигли ли цели
        if (Vector3.Distance(transform.position, currentTarget.position) < 1f)
        {
            SetNewTarget(); // Переходим к новой цели
        }
    }

    // Установка новой цели
    void SetNewTarget()
    {
        currentTarget = navigationManager.GetRandomNavPoint();
    }
}