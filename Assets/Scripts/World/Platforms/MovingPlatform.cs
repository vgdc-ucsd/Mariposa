using System.Collections.Generic;
using UnityEngine;

public abstract class MovingPlatform : MonoBehaviour
{
    protected Vector2[] pathNodes;
    protected int nodeCount;
    protected Rigidbody2D platformRb;

    protected int currNodeIdx = 0;
    protected int nextNodeIdx = 1;
    public Vector2 velocity = Vector2.zero;

    [SerializeField] protected float platformMoveSpeed;
    [SerializeField] protected bool isLooping;

    protected virtual void Awake()
    {
        platformRb = transform.GetChild(0).GetComponent<Rigidbody2D>();

        Transform pathNodeContainer = transform.GetChild(1);

        if (pathNodeContainer.childCount < 2) Debug.LogError(name + " is missing path nodes");

        nodeCount = pathNodeContainer.childCount;
        pathNodes = new Vector2[nodeCount];
        int i = 0;
        foreach (Transform child in pathNodeContainer) {
            pathNodes[i] = child.position;
            ++i;
        }

        nextNodeIdx = GetNextNodeIdx(currNodeIdx);

        platformRb.MovePosition(pathNodes[0]);
    }

    protected int GetNextNodeIdx(int nodeIdx)
    {
        return (nodeIdx + 1) % nodeCount;
    }

    protected virtual void FixedUpdate()
    {
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

        velocity = movement / fdt;
        platformRb.MovePosition(platformRb.position + movement);
    }
}
