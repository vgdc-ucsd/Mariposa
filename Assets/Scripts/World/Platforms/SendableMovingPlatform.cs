using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class SendableMovingPlatform : MovingPlatform
{
    public override void Initialize()
    {
        base.Initialize();

        currentNodeIdx = initialNodeIndex;
    }

    /// <summary>
    /// Sends the platform to move <paramref name="nodesToTraverse"/> nodes. If <paramref name="nodesToTraverse"/>
    /// is negative, will move the platform down.
    /// </summary>
    /// <param name="nodesToTraverse">The number of nodes to traverse</param>
    public void SendPlatform(int nodesToTraverse)
    {
        if (state == PlatformState.Resetting) return;

        if (nodesToTraverse == 0) return;

        int targetNodeIdx = currentNodeIdx + nodesToTraverse;

        if (targetNodeIdx < 0 || targetNodeIdx >= manager.pathNodes.Length)
        {
            return;
        }
        if (nodesToTraverse > 0)
        {
            for (int i = currentNodeIdx + 1; i <= targetNodeIdx; ++i)
            {
                if (manager.platforms[i] != null) return;
            }
        }
        else
        {
            for (int i = currentNodeIdx - 1; i >= targetNodeIdx; --i)
            {
                if (manager.platforms[i] != null) return;
            }
        }

        manager.platforms[currentNodeIdx] = null;
        manager.platforms[targetNodeIdx] = this;
        currentNodeIdx = targetNodeIdx;

        state = PlatformState.Moving;
    }

    protected override Vector2 GetCurrentMovement()
    {
        float fdt = Time.fixedDeltaTime;

        Vector2 targetPos = manager.pathNodes[currentNodeIdx];
        Vector2 separationToTarget = targetPos - rb.position;

        if (separationToTarget.magnitude > platformMoveSpeed * fdt)
        {
            return platformMoveSpeed * fdt * separationToTarget.normalized;
        }
        else
        {
            StopMoving(currentNodeIdx);
            return separationToTarget;
        }
    }

    protected override void StopMoving(int stopNodeIdx)
    {
        state = PlatformState.Stopped;
        currentNodeIdx = stopNodeIdx;
    }
}
