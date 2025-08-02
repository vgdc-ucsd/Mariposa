using UnityEngine;
using UnityEngine.UI;

public class ParallaxScroll : MonoBehaviour
{
    public float parallaxEffect;
    [SerializeField] private float yOffset = 0f;
    private float startPos;
    private float length;
    private float horizontalDisplacement;

    void OnEnable()
    {
        // rectTransform.position = new(Camera.main.transform.position.x, rectTransform.position.y, rectTransform.position.z);
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        // new implementation; horizontally displace item from canvas, which moves with camera
        float distance = Camera.main.transform.position.x * parallaxEffect;
        float movement = Camera.main.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, Camera.main.transform.position.y + yOffset, transform.position.z);
        
        if (movement > (startPos + length))
        {
            startPos += length;
        }
        else if (movement < (startPos - length))
        {
            startPos -= length;
        }
    }
}
