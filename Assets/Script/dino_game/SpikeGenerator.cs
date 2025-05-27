using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeGenerator : MonoBehaviour
{
    public List<GameObject> spikes;
    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;
    public float tims = 2;
    public float speed_sc = 0;

    private void Awake()
    {
        currentSpeed = minSpeed;
        GenerateSpike();
    }

    public void GenerateSpike()
    {
        int rn = Random.Range(0, spikes.Count); 
        Debug.Log("gener");
        GameObject spikeInstance = Instantiate(spikes[rn], transform.position, transform.rotation);

        spikeInstance.GetComponent<SpikeScript>().spikeGenerator=this;
    }

    public void Update()
    {
        tims-=Time.deltaTime;
        speed_sc+=Time.deltaTime;
        if (tims <= 0)
        {
            GenerateSpike();
            if (speed_sc >= 60)
                tims = 1.1f;
            else
            {
                if (speed_sc >= 30)
                    tims = 1.6f;
                else 
                    tims = 2;
            }
        }
    }
    
    
    public void Restart()
    {
        // Удаляем все шипы из сцены
        SpikeScript[] activeSpikes = FindObjectsOfType<SpikeScript>();
        foreach (SpikeScript spike in activeSpikes)
        {
            Destroy(spike.gameObject);
        }

        // Сбрасываем параметры генерации
        currentSpeed = minSpeed;
        tims = 2f;
        speed_sc = 0f;

        // Генерируем первый шип заново
        GenerateSpike();
    }

    
       
    
}

