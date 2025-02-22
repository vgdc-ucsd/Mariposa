using UnityEngine;

public class GrappleTarget : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;

    public void ToggleHighlight(bool toggle)
    {
        sr.color = toggle ? Color.green : Color.yellow;
    }
}
