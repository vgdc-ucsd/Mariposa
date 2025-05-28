using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;


public abstract class MovingPlatform : MonoBehaviour
{
    public enum PlatformState
    {
        Stopped,
        Moving,
        Resetting,
    }

    public Rigidbody2D rb { get; private set; }
    public BoxCollider2D col { get; private set; }
    public MovingPlatformManager manager;

    public int initialNodeIndex;
    public int currentNodeIdx;
    public PlatformState state;
    public FreeBody adjacentFreeBody;
    public Vector2 currentMovement;

    [SerializeField] protected LayerMask playerLayerMask;

    [SerializeField] protected float platformMoveSpeed = 5f;
    [SerializeField] protected int physicsFramesToPushPlayerOut = 4;

    public virtual void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        state = PlatformState.Stopped;
        adjacentFreeBody = null;
        currentMovement = Vector2.zero;
        initialNodeIndex = GetInitialNodeIndex();
        currentNodeIdx = initialNodeIndex;
    }

    public virtual int GetInitialNodeIndex()
    {
        return manager.GetClosestNodeIndex(rb.position);
    }


    protected abstract Vector2 GetCurrentMovement();

    protected Vector2 GetResettingMovement()
    {
        float fdt = Time.fixedDeltaTime;

        manager.platforms[currentNodeIdx] = null;
        manager.platforms[initialNodeIndex] = this;

        Vector2 initialNode = manager.pathNodes[initialNodeIndex];
        Vector2 separationToInitialNode = initialNode - rb.position;

        if (separationToInitialNode.magnitude > platformMoveSpeed * fdt)
        {
            return platformMoveSpeed * fdt * separationToInitialNode.normalized;
        }
        else
        {
            StopMoving(initialNodeIndex);
            return separationToInitialNode;
        }
    }

    protected virtual void StopMoving(int stopNodeIdx)
    {
        state = PlatformState.Stopped;
        for (int i = 0; i < manager.platforms.Length; ++i)
        {
            MovingPlatform platform = manager.platforms[i];
            if (platform != null && platform == this) manager.platforms[i] = null;
        }
        manager.platforms[stopNodeIdx] = this;
    }

    protected virtual void PushPlayerOut()
    {
        Vector2 pushColOrigin = (Vector2)col.bounds.center + col.bounds.extents.y * Vector2.down;
        Vector2 pushColSize = new(col.size.x, col.size.y / 2);
        float pushFraction = 1 / (float)physicsFramesToPushPlayerOut;

        Collider2D playerCol = Physics2D.OverlapBox(pushColOrigin, pushColSize, 0f, playerLayerMask);
        if (!playerCol) return;

        playerCol.transform.position += playerCol.transform.position.x > transform.position.x
            ? (col.bounds.max.x - playerCol.bounds.min.x + FreeBody.CONTACT_OFFSET) * pushFraction * Vector3.right
            : (playerCol.bounds.max.x - col.bounds.min.x + FreeBody.CONTACT_OFFSET) * pushFraction * Vector3.left;
    }

    protected virtual void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        switch (state)
        {
            case PlatformState.Moving:
                currentMovement = GetCurrentMovement();
                break;
            case PlatformState.Resetting:
                currentMovement = GetResettingMovement();
                break;
            case PlatformState.Stopped:
                return;
            default:
                Debug.LogError(rb.gameObject.name + " has an invalid state");
                return;
        }

        rb.transform.position += (Vector3)currentMovement;
        Physics2D.SyncTransforms();

        if (adjacentFreeBody != null)
        {
            Vector2 movementToApply = currentMovement;
            if (currentMovement.y / fdt < -adjacentFreeBody.TerminalVelocity) 
                movementToApply.y = -adjacentFreeBody.TerminalVelocity * fdt; // cap the free body's downward movement;
            adjacentFreeBody.transform.position += (Vector3)movementToApply;
        }

        PushPlayerOut();
    }
}
