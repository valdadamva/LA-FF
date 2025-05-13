using UnityEngine;

public class SpikeGenerator : MonoBehaviour
{
    public GameObject spike;
    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;

    private void Awake()
    {
        currentSpeed = minSpeed;
        GenerateSpike();
    }

    public void GenerateSpike()
    {
        GameObject spikeInstance = Instantiate(spike, transform.position, transform.rotation);

        spikeInstance.GetComponent<SpikeScript>().spikeGenerator=this;
    }
}

