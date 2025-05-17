using UnityEngine;

public class GrappleTarget : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] bool oneUse;

    public void ToggleHighlight(bool toggle)
    {
        sr.color = toggle ? Color.green : Color.yellow;
    }

    public virtual void ReleaseGrapple()
    {
        if (oneUse)
        {
            GrappleAbility.Instance.RemoveGrappleTarget(this);
            Destroy(gameObject);
        }
    }
}
