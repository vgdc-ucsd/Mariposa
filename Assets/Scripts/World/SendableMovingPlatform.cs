using UnityEngine;

public class SendableMovingPlatform : MovingPlatform
{
    private bool isMoving = false;

    [ContextMenu("SendPlatform")]
    public void SendPlatform()
    {
        isMoving = true;
        nextNodeIdx = GetNextNodeIdx(currNodeIdx);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        if (!isMoving)
        {
            velocity = Vector2.zero;
            return;
        }

        float fdt = Time.fixedDeltaTime;
        Vector2 nextNode = pathNodes[nextNodeIdx];
        Vector2 separationToNextNode = nextNode - platformRb.position;

        Vector2 movement = platformMoveSpeed * fdt * separationToNextNode.normalized;
        float distanceToNextNode = separationToNextNode.magnitude;
        if (distanceToNextNode < platformMoveSpeed * fdt)
        {
            currNodeIdx = nextNodeIdx;
            nextNodeIdx = (currNodeIdx + 1) % nodeCount;
            movement = separationToNextNode;
            isMoving = false;
        }

        velocity = movement / fdt;
        platformRb.MovePosition(platformRb.position + movement);
    }
}
