using JetBrains.Annotations;
using UnityEngine;

public class AutomaticMovingPlatform : MovingPlatform
{
    [SerializeField] private bool isLooping;
    private bool isMovingForward = true;

    private int nextNodeIdx;

    public override void Initialize()
    {
        base.Initialize();

        nextNodeIdx = 1;
        isMovingForward = true;
        state = PlatformState.Moving;
    }

    protected override Vector2 GetCurrentMovement()
    {
        float fdt = Time.fixedDeltaTime;

        Vector2 targetNode = manager.pathNodes[nextNodeIdx];
        Vector2 separationToTarget = targetNode - rb.position;

        if (separationToTarget.magnitude < platformMoveSpeed * fdt)
        {
            SetNextNodeIndex();

            /*
            float fraction = separationToTarget.magnitude / (platformMoveSpeed * fdt);
            Vector2 separationToNextTarget = manager.pathNodes[nextNodeIdx] - targetNode;
            Vector2 moveDir = fraction * separationToTarget + (1 - fraction) * separationToNextTarget;
            return platformMoveSpeed * fdt * moveDir;
            */
        }
        return platformMoveSpeed * fdt * separationToTarget.normalized;
    }

    public void SetNextNodeIndex()
    {
        if (isLooping)
        {
            nextNodeIdx = (nextNodeIdx + 1) % manager.pathNodes.Length;
        }
        else
        {
            if (isMovingForward)
            {
                nextNodeIdx++;
                if (nextNodeIdx == manager.pathNodes.Length)
                {
                    nextNodeIdx -= 2;
                    isMovingForward = false;
                }
            }
            else
            {
                nextNodeIdx--;
                if (nextNodeIdx == -1)
                {
                    nextNodeIdx += 2;
                    isMovingForward = true;
                }
            }
        }
    }

    protected override void PushPlayerOut()
    {
        Vector2 pushColOrigin = (Vector2)col.bounds.center + col.bounds.extents.y * Vector2.down;
        Vector2 pushColSize = new(col.size.x, col.size.y / 2);

        Collider2D playerCol = Physics2D.OverlapBox(pushColOrigin, pushColSize, 0f, playerLayerMask);
        if (!playerCol) return;

        FreeBody player = playerCol.GetComponent<FreeBody>();
        if (player.State == BodyState.OnGround && currentMovement.y < 0) base.PushPlayerOut();
        else player.ApplyMovement(currentMovement);
    }
}
