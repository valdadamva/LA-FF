using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public float JumpForce;
	public float tim=0;
	 public TMP_Text survivalTimeText;
	private bool isGameOver = false;

    [SerializeField]
    bool isGrounded = false;

    Rigidbody RB;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
		if (!isGameOver)
			tim+=Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isGrounded)
            {
                RB.AddForce(Vector2.up * JumpForce);
                isGrounded = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("sole"))
        {
            Debug.Log("Collided");
            if(!isGrounded )
            {
                isGrounded = true;
            }
        }
    }

	public void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (survivalTimeText != null)
        {
            survivalTimeText.gameObject.SetActive(true);
			float k= tim/3;
            int y = (int)k;
			if (y>20)
				y=20;
			string i=y.ToString();
            survivalTimeText.text = "Ta note: "+i+"/20";
        }
    }
 
	
}