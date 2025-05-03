using UnityEditor;
using UnityEngine;

public class SendableMovingPlatform : MovingPlatform
{
    private bool isMoving = false;
    [SerializeField] private int physicsFramesToPushPlayerOut = 4;

    [ContextMenu("SendPlatform")]
    public void SendPlatform()
    {
        if (isMoving)
        {
            isMovingForward = !isMovingForward;
            MoveToNextTarget();
        }
        isMoving = true;

        Vector2 targetNode = pathNodes[targetNodeIdx];
        Vector2 separationToNextNode = targetNode - platformRb.position;
        currMovement = platformMoveSpeed * Time.fixedDeltaTime * separationToNextNode.normalized;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        if (!isMoving)
        {
            currMovement = Vector2.zero;
            return;
        }

        float fdt = Time.fixedDeltaTime;

        Vector2 targetNode = pathNodes[targetNodeIdx];
        Vector2 separationToNextNode = targetNode - platformRb.position;

        Vector2 movement = platformMoveSpeed * fdt * separationToNextNode.normalized;
        float distanceToNextNode = separationToNextNode.magnitude;
        if (distanceToNextNode < platformMoveSpeed * fdt)
        {
            movement = separationToNextNode;

            if (MoveToNextTarget()) isMoving = false;
        }

        currMovement = movement;
        platformRb.transform.position += (Vector3)movement;
        Physics2D.SyncTransforms();

        if (adjacentFreeBody != null)
        {
            Vector2 movementToApply = movement;
            if (currMovement.y / fdt < -adjacentFreeBody.TerminalVelocity) 
                movementToApply.y = -adjacentFreeBody.TerminalVelocity * fdt; // cap the free body's downward movement;
            adjacentFreeBody.transform.position += (Vector3)movementToApply;
        }

        if (movement.y >= 0) return;
        Vector2 pushColOrigin = (Vector2)platformCol.bounds.center + platformCol.bounds.extents.y * Vector2.down;
        Vector2 pushColSize = new(platformCol.size.x, platformCol.size.y / 2);
        Collider2D playerCol = Physics2D.OverlapBox(pushColOrigin, pushColSize, 0f, playerLayerMask);
        float pushFraction = 1 / (float)physicsFramesToPushPlayerOut;
        if (!playerCol) return;
        playerCol.transform.position += playerCol.transform.position.x > transform.position.x
            ? (platformCol.bounds.max.x - playerCol.bounds.min.x + FreeBody.CONTACT_OFFSET) * pushFraction * Vector3.right
            : (playerCol.bounds.max.x - platformCol.bounds.min.x + FreeBody.CONTACT_OFFSET) * pushFraction * Vector3.left;
    }
}
