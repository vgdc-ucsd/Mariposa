using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    public float parallaxEffect;
    [SerializeField] private float yOffset = 0f;
    private float startPos;
    private float length;

    void OnEnable()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float distance = Camera.main.transform.position.x * parallaxEffect;
        float movement = Camera.main.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, Camera.main.transform.position.y + yOffset, transform.position.z);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
