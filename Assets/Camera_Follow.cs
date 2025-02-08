using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f); //defines the offset of the camera to the player
    private float followTime = 0.20f; //time it takes for the camera to reach the player
    private Vector3 velocity = Vector3.zero; //velocity

    [SerializeField] private Transform target; //allows us to set the target of the camera in the editor.


    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followTime);
        // params in order: camera position, target position, reference to the velocity, following time
    }
    
}
