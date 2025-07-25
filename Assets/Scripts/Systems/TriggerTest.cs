using UnityEngine;

// used to test triggers and colliders
public class TriggerTest : MonoBehaviour
{
    private GameObject obj;

    void Start()
    {
        obj = this.gameObject;
        Debug.Log(obj + "trigger test activated");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision triggered");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger triggered");
    }
}
