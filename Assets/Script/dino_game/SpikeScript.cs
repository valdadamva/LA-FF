using UnityEngine;

public class SpikeScript : MonoBehaviour
{
	public PlayerScript ps;

    [Header("References")]
    public SpikeGenerator spikeGenerator; 
   
    private void Update()
    {
        // Движение шипа влево
        if (spikeGenerator != null)
        {
            transform.Translate(Vector2.left * spikeGenerator.currentSpeed * Time.deltaTime);
        }
    }

    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision == null)
        {
            Debug.Log("Null Collider");
            return;
        }
        
		if (collision.CompareTag("sole"))
		{   
                 Destroy(this.gameObject);  
        }
        if(collision.gameObject.CompareTag("enemie"))
        {
            Debug.Log("fin");
			ps = FindObjectOfType<PlayerScript>();
            ps.EndGame();
        }
    }
}