using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Background Settings")]
    public float speed = 2f;
    public float width;
    public bool isTile = false;
    public float chunkWidth = 20f;

    void Start()
    {
        if (!isTile)
        {
            width = GetComponent<SpriteRenderer>().bounds.size.x;

        }

    }
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (!isTile)
        {
            if (transform.position.x <= -width)
            {
                transform.position += Vector3.right * width * 2f;
            }
        }
        else
        {

            if (transform.position.x <= -chunkWidth)
            {
                transform.position += Vector3.right * chunkWidth * 2f;
            }
        }
    }
        
    
}
