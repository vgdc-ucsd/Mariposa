using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos, length;
    public GameObject Cam;       // Bind the main camera 
    public float ParallaxEffect; // [0, 1], the smaller the faster

    
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }


    void FixedUpdate()
    {
        float camPos = Cam.transform.position.x;
        float bgPos = transform.position.x;

        // Calculate the position of the background moves based on cam movement
        float distance = camPos * ParallaxEffect; 
        transform.position = new Vector3(startPos + distance, transform.position.y, 
                    transform.position.z);

        
        // If the camera moves beyond the bound of the background
        // reposition the background to the position of the camera. 
        if (camPos > bgPos + length)
        {
            startPos += length;
        } else if(camPos < bgPos - length)
        {
            startPos -= length;
        }
    }
}
