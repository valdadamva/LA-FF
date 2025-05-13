using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private GameObject deleteBackground;
    [SerializeField] private GameObject createBackground;
    public float speed;
    public static bool activ;

    private Transform myTransform;
    private Transform deleteBackgroundTransform;
    private Transform createBackgroundTransform;

    private void Start()
    {
        activ = true;
        myTransform = this.transform;
        deleteBackgroundTransform = deleteBackground.transform;
        createBackgroundTransform = createBackground.transform;
    }

    void FixedUpdate()
    {
        if (activ == true)
        {
            myTransform.position = new Vector2((myTransform.position.x - speed * Time.deltaTime), myTransform.localPosition.y);
            
            if (myTransform.localPosition.x < deleteBackgroundTransform.localPosition.x)
            {
                myTransform.position = new Vector2(createBackgroundTransform.position.x, myTransform.position.y);
            }
        }
    }
}  
