using UnityEngine;

public interface IBeeBehavior
{
    // Specifies a vector to add to the position in the next frame
    public Vector2 GetMoveStep(float fdt);

    // used to determine which direction the bee should face while auto moving
    public Vector2 GetDir();
}

public class Follow : IBeeBehavior
{
    private Transform origin;
    private Transform target;
    private float speed = 1f;
    private float distance = 3f;

    public Follow(Transform origin, Transform target, float speed = 1f, float distance = 3f)
    {
        this.origin = origin;
        this.target = target;
        this.speed = speed;
        this.distance = distance;
    }

    public Vector2 GetMoveStep(float fdt)
    {
        if (Vector2.Distance(origin.position, target.position) < distance)
        {
            return Vector2.zero;
        }
        return (target.position - origin.position) * speed * fdt;
    }

    public Vector2 GetDir() {
        Vector2 direction2D = (Vector2)(target.position - origin.position);
        direction2D.Normalize();
        return direction2D;
    }
}
