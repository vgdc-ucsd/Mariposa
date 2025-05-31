using System;
using UnityEngine;

public class MovingPlatformManager : MonoBehaviour
{
    [SerializeField] private Transform pathNodeContainer;
    [SerializeField] private Transform platformContainer;
    public Vector2[] pathNodes;
    public MovingPlatform[] platforms;

    [SerializeField] private bool showPath = true;

    private void Awake()
    {
        InitializePathNodes();
        InitializePlatforms();
    }

    [ContextMenu("Initialize Path Nodes")]
    public void InitializePathNodes()
    {
        if (pathNodeContainer.childCount < 2) Debug.LogError(name + " is missing path nodes. It should have at least two in its path node container.");

        pathNodes = new Vector2[pathNodeContainer.childCount];
        int i = 0;
        foreach (Transform child in pathNodeContainer) {
            pathNodes[i] = child.position;
            ++i;
        }
    }

    [ContextMenu("Initialize Platforms")]
    public void InitializePlatforms()
    {
        if (platformContainer.childCount < 1) Debug.LogError(name + " is missing a platform.");

        platforms = new MovingPlatform[pathNodeContainer.childCount];
        platforms.Initialize();

        foreach (Transform child in platformContainer)
        {
            MovingPlatform platform = child.GetComponent<MovingPlatform>();

            platform.Initialize();
            if (platform.manager == null || platform.manager != this) platform.manager = this;
            platform.transform.position = pathNodes[platform.initialNodeIndex];

            if (platforms[platform.initialNodeIndex] != null)
            {
                Debug.LogError(platform.gameObject.name
                    + " cannot be in the same position as "
                    + platforms[platform.initialNodeIndex].gameObject.name);
            }
            platforms[platform.initialNodeIndex] = platform;
        }
    }

    public int ClampNodeIndex(int index) => Math.Clamp(index, 0, pathNodes.Length - 1);

    public int GetClosestNodeIndex(Vector2 position)
    {
        int initialNodeIndex = 0;
        float minSqDistance = Mathf.Infinity;
        Vector2 platformPos = position;
        for (int i = 0; i < pathNodes.Length; ++i)
        {
            Vector2 nodePos = pathNodes[i];
            float currSqDistance = (platformPos - nodePos).sqrMagnitude;
            if (currSqDistance < minSqDistance)
            {
                minSqDistance = currSqDistance;
                initialNodeIndex = i;
            }
        }

        return initialNodeIndex;
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
}
