using UnityEngine;

public class SpikeGenerator : MonoBehaviour
{
    public GameObject spike;
    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;
    public float tims = 2;

    private void Awake()
    {
        currentSpeed = minSpeed;
        GenerateSpike();
    }

    public void GenerateSpike()
    {
        
        
        Debug.Log("gener");
        GameObject spikeInstance = Instantiate(spike, transform.position, transform.rotation);

        spikeInstance.GetComponent<SpikeScript>().spikeGenerator=this;
    }

    public void Update()
    {
        tims-=Time.deltaTime;
        if (tims <= 0)
        {
            GenerateSpike();
            tims = 2;
        }
    }
    
       
    
}

