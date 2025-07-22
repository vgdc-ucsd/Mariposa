using UnityEngine;
using Unity.VisualScripting;
using System;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed;

    public int bounces;
    public int pierce;
    [SerializeField] protected Vector2 direction;
    public float lifetimeCtr;
    public float lifetimeDuration;

    [SerializeField] protected Vector2 velocity;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected CircleCollider2D circleCol;

    [SerializeField] protected bool destroyNow;
    [SerializeField] protected GameObject player;

    void Awake()
    {
        lifetimeCtr = 0.0f;
        destroyNow = false;
        InitColliders();
    }

    void Update()
    {
        if (!isActiveAndEnabled) return;

        float dt = Time.deltaTime;
        lifetimeCtr += dt;

        UpdateBehavior(dt);

        // destroy conditions
        if (lifetimeCtr > lifetimeDuration || pierce < 1 || bounces < 1)
        {
            destroyNow = true;
        }

        if (destroyNow)
        {
            Kill();
        }
    }

    public virtual void UpdateBehavior(float dt)
    {

    }

    public void SetVelocityDirection(Vector2 direction)
    {
        velocity = speed * direction.normalized;
    }

    public void RotateVelocityDirection(float rads)
    {
        velocity = new((float)(velocity.x * Math.Cos((double)rads) - velocity.y * Math.Sin((double)rads)),  (float)(velocity.x * Math.Sin((double)rads) + velocity.y * Math.Cos((double)rads)));
    }

    public void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
        velocity = velocity.normalized * speed;
    }

    public void InitColliders()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        circleCol = this.gameObject.GetComponent<CircleCollider2D>();
    }

    public virtual void Initialize(Vector3 position, Vector2 direction)
    {
        this.lifetimeCtr = 0.0f;

        this.transform.position = position;
        this.direction = direction.normalized;

        this.velocity = direction.normalized * speed;
    }
    
    public void Kill()
    {
        OnKill();
        Destroy(this.gameObject);
    }

    public virtual void OnKill()
    {
        
    }
}
