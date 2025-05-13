using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    [Header("References")]
    public SpikeGenerator spikeGenerator; // Ссылка на генератор

    private void Update()
    {
        // Движение шипа влево
        if (spikeGenerator != null)
        {
            transform.Translate(Vector2.left * spikeGenerator.currentSpeed * Time.deltaTime);
        }
    }

    // Правильная сигнатура метода для 2D триггера
    private void OnTriggerEnter(Collider collision)
    {
        if (collision == null)
        {
            Debug.Log("Null Collider");
            return;
        }
        
        if (collision.CompareTag("ABC"))
        {Debug.Log("ABC");
            if (spikeGenerator != null)
            {
                Debug.Log("ABC_true");
                spikeGenerator.GenerateSpike();
            }
        }
    }
}