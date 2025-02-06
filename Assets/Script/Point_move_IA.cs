using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public Transform[] navPoints; // Массив точек навигации

    // Получение случайной точки навигации
    public Transform GetRandomNavPoint()
    {
        if (navPoints.Length == 0)
        {
            Debug.LogError("Нет точек навигации!");
            return null;
        }

        int randomIndex = Random.Range(0, navPoints.Length);
        return navPoints[randomIndex];
    }
}