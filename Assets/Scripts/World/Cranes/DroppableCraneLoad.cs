using UnityEngine;

public class DroppableCraneLoad : CraneLoad
{
    int entityLayer;
    int barrierLayer;
    void Start()
    {
        entityLayer = LayerMask.NameToLayer("Entity");
        barrierLayer = LayerMask.NameToLayer("Barrier");
    }

    // destroy any entity or object it touches
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidedObj = collision.gameObject;
        if (collidedObj.CompareTag("CrushableObject") || collidedObj.layer == entityLayer)
        {
            // maybe an explosion effect here
            Destroy(collidedObj);
        }
    }
}
