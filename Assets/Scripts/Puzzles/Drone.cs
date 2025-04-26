using UnityEngine;
// enemy drone class : kills player if in the drone's vision
public class Drone : MonoBehaviour
{
  
    public  Player player;
    public float fovAngle;
    public Vector3 targetPosition;
    public Vector3 lastPlayerPosition;
    public bool inSight;

    // checks if the player is in sight of drone
    public bool PlayerInSight()
    {
        Vector3 toTarget = player.transform.position-transform.position;
        if((Vector3.Angle(transform.forward, toTarget)<=fovAngle))
        {
            targetPosition = player.transform.position;
            lastPlayerPosition = targetPosition;
            inSight=true;
            return inSight;

        }
        inSight=false;
        return inSight;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
