using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static UnityEngine.UI.Image;

public interface IBeeBehavior
{
    // Specifies a vector to add to the position in the next frame
    public Vector2 GetMoveStep(float fdt);

    public Vector2 GetDir()
    {
        Vector2 direction2D = (Vector2)(Player.ActivePlayer.transform.position - Bee.Instance.transform.position);
        direction2D.Normalize();
        return direction2D;
        
    }
}

public class Follow : IBeeBehavior
{
    Transform origin, target;
    private float speed = 1f;
    private float distance = 3f;

    public Follow(float speed = 1f, float distance = 3f)
    {
        origin = Bee.Instance.transform;
        target = Player.ActivePlayer.transform;
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

    // default: face player
    public Vector2 GetDir() {
        Vector2 direction2D = (Vector2)(target.position - origin.position);
        direction2D.Normalize();
        return direction2D;
    }
}

public class Stay : IBeeBehavior
{
    public Vector2 GetMoveStep(float fdt)
    {
        return Vector2.zero;
    }
}

public class JumpAssist : IBeeBehavior
{
    Transform origin, target;
    private const float speed = 20f;
    private bool stopping;
    private float timeRemaining = 0.15f;

    private Vector2 finalPos;
    
    public JumpAssist()
    {
        origin = Bee.Instance.transform;
        target = Player.ActivePlayer.transform;
    }


    public Vector2 GetMoveStep(float fdt)
    {
        Vector2 predictedPosition = (Vector2)target.position + Player.ActivePlayer.Movement.Velocity * timeRemaining;

        if (!stopping && timeRemaining < 0)
        {
            stopping = true;
            finalPos = (Vector2)target.position;
            Bee.Instance.Movement.StartCoroutine(Finish());
        }
        Vector2 moveTarget;
        if (stopping)
        {
            moveTarget = finalPos + new Vector2(0, -1.5f);
        }
        else
        {
            moveTarget = predictedPosition + new Vector2(0, -1.5f);
        }

        var newPos = Vector2.MoveTowards(origin.position, moveTarget, speed * fdt);
        timeRemaining -= fdt;
        return newPos - (Vector2)origin.position;
    }


    private IEnumerator Finish()
    {
        yield return new WaitForSeconds(0.2f);
        Bee.Instance.StartFollow();
    }
}