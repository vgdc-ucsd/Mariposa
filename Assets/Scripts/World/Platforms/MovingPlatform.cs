using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MovingPlatform : MonoBehaviour
{
    protected Vector2[] pathNodes;
    protected int nodeCount;
    public Rigidbody2D platformRb;
    public BoxCollider2D platformCol;
    [SerializeField] protected Transform pathNodeContainer;
    [SerializeField] protected LayerMask playerLayerMask;
    public FreeBody adjacentFreeBody = null;

    protected int targetNodeIdx;
    protected bool isMovingForward = true;
    /// <summary>
    /// The platform's movement for the current physics frame;
    /// </summary>
    public Vector2 currMovement = Vector2.zero;

    [SerializeField] protected float platformMoveSpeed;
    [SerializeField] protected bool isLooping;

    [SerializeField] private bool showPath = true;

    protected virtual void Awake()
    {
        InitializePathNodes();

        targetNodeIdx = 1;

        platformRb.MovePosition(pathNodes[0]);
    }

    private const float NODE_VISUAL_RADIUS = 0.2f;
    private void OnDrawGizmos()
    {
        if (!Application.isEditor || !showPath || pathNodeContainer.childCount < 2) return;

        Vector2[] nodes = new Vector2[pathNodeContainer.childCount];
        for (int i = 0; i < pathNodeContainer.childCount; ++i)
        {
            nodes[i] = pathNodeContainer.GetChild(i).position;
        }
        for (int i = 0; i < nodes.Length - 1; ++i)
        {
            Gizmos.DrawSphere(nodes[i], NODE_VISUAL_RADIUS);
            Gizmos.DrawLine(nodes[i], nodes[i + 1]);
        }
        Gizmos.DrawSphere(nodes[^1], NODE_VISUAL_RADIUS);
    }

    [ContextMenu("Initialize Path Nodes")]
    public void InitializePathNodes()
    {
        if (pathNodeContainer.childCount < 2) Debug.LogError(name + " is missing path nodes. It should have at least two in its path node container.");

        nodeCount = pathNodeContainer.childCount;
        pathNodes = new Vector2[nodeCount];
        int i = 0;
        foreach (Transform child in pathNodeContainer) {
            pathNodes[i] = child.position;
            ++i;
        }
    }

    /// <summary>
    /// Changes the target node to the next; Will switch direction if this
    /// is a non-looping moving platform.
    /// </summary>
    /// <returns>True if this moving platform reached the end of its path
    /// and false otherwise</returns>
    protected bool MoveToNextTarget()
    {
        bool didReachEnd = false;
        if (!isLooping)
        {
            if (targetNodeIdx == pathNodes.Length - 1)
            {
                isMovingForward = false;
                didReachEnd = true;
            }
            else if (targetNodeIdx == 0)
            {
                isMovingForward = true;
                didReachEnd = true;
            }

            targetNodeIdx = isMovingForward
                ? targetNodeIdx + 1
                : targetNodeIdx - 1;
        }
        else
        {
            if (targetNodeIdx == 0) didReachEnd = true;
            targetNodeIdx = (targetNodeIdx + 1) % nodeCount;
        }
        return didReachEnd;
    }

    protected virtual void FixedUpdate()
    {
        /*
        float fdt = Time.fixedDeltaTime;
        Vector2 nextNode = pathNodes[nextNodeIdx];
        Vector2 separationToNextNode = nextNode - platformRb.position;

        Vector2 movement = platformMoveSpeed * fdt * separationToNextNode.normalized;
        float distanceToNextNode = separationToNextNode.magnitude;
        if (distanceToNextNode < platformMoveSpeed * fdt)
        {
            currNodeIdx = nextNodeIdx;
            nextNodeIdx = (currNodeIdx + 1) % nodeCount;
            Vector2 nextNextNode = pathNodes[nextNodeIdx];
            Vector2 newPosition = nextNode + (platformMoveSpeed * fdt - distanceToNextNode) * (nextNextNode - nextNode).normalized;
            movement = newPosition - platformRb.position;
        }

        currMovement = movement;
        platformRb.MovePosition(platformRb.position + movement);
        */
    }
}
