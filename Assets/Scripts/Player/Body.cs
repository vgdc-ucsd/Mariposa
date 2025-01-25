using UnityEngine;

// Anything that can move, and needs to keep track of velocity
public abstract class Body : MonoBehaviour
{
    public Vector2 Velocity;
    public Rigidbody2D Rb;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        transform.position += (Vector3)Velocity * Time.deltaTime;
    }
}