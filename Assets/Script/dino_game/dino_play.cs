using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float JumpForce;

    [SerializeField]
    bool isGrounded = false;

    Rigidbody RB;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
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
}