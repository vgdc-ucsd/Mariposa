using UnityEngine;

public class BallProjectile : Projectile
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(collision.gameObject.GetComponent<Player>().Die());
            pierce--;
        }
        else
        {
            bounces--;
        }
    }

    public override void UpdateBehavior(float dt)
    {
        if (velocity != Vector2.zero)
        {
            transform.position += (Vector3)(velocity * dt);
        }
    }
}
