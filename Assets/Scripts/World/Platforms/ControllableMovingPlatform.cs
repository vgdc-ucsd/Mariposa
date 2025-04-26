using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ControllableMovingPlatform : MovingPlatform
{
    private Vector2 startNode;
    private Vector2 endNode;
    private Vector2 path;
    private Vector2 moveDir = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        if (pathNodes.Length != 2) Debug.LogError(name + " should have exactly 2 path nodes, but it has " + pathNodes.Length);
        if (platformMoveSpeed == 0) platformMoveSpeed = 1f;
        startNode = pathNodes[0];
        endNode = pathNodes[1];
        path = endNode - startNode;
    }

    public void MovePlatform(Vector2 controlDir)
    {
        moveDir = Helper.Vec2Proj(controlDir, path).normalized;
    }

    protected override void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;
        Vector2 targetPos = platformRb.position + platformMoveSpeed * fdt * moveDir;
        if ((targetPos - endNode).sqrMagnitude > path.sqrMagnitude)
        {
            targetPos = startNode;
        }
        else if ((targetPos - startNode).sqrMagnitude > path.sqrMagnitude)
        {
            targetPos = endNode;
        }
        velocity = targetPos - platformRb.position;
        platformRb.MovePosition(targetPos);
    }
}
