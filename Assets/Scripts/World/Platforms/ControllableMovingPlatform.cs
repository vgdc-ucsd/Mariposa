using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ControllableMovingPlatform : MovingPlatform
{
    private Vector2 startNode;
    private Vector2 endNode;
    private Vector2 path;
    private Vector2 moveDir = Vector2.zero;

    public override void Initialize()
    {
        base.Initialize();

        startNode = manager.pathNodes[0];
        endNode = manager.pathNodes[^1];
        path = endNode - startNode;
    }

    public override int GetInitialNodeIndex() => 0;

    public void MovePlatform(Vector2 controlDir)
    {
        if (state == PlatformState.Resetting) return;

        if (controlDir == Vector2.zero)
        {
            state = PlatformState.Stopped;
        }
        else
        {
            moveDir = Helper.Vec2Proj(controlDir, path).normalized;
            state = PlatformState.Moving;
        }
    }

    protected override Vector2 GetCurrentMovement()
    {
        float fdt = Time.fixedDeltaTime;

        Vector2 targetPos = rb.position + platformMoveSpeed * fdt * moveDir;

        for (int i = 0; i < manager.pathNodes.Length; ++i)
        {
            Vector2 node = manager.pathNodes[i];
            if (Vector2.Distance(node, rb.position) < 0.1f) currentNodeIdx = i;
        }

        if ((targetPos - endNode).sqrMagnitude > path.sqrMagnitude)
        {
            targetPos = startNode;
            StopMoving(0);
        }
        else if ((targetPos - startNode).sqrMagnitude > path.sqrMagnitude)
        {
            targetPos = endNode;
            StopMoving(manager.pathNodes.Length - 1);
        }

        Vector2 movement = targetPos - rb.position;

        return movement;
    }
}
