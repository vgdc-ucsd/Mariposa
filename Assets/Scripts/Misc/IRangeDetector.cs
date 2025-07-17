using UnityEngine;

public interface IRangeDetector
{
    public void SetTarget(GameObject newTarget);
    public bool IsTargetInRange();
    public float GetMaxLength(Vector2 direction);
}
