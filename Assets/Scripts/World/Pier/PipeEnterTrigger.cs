using UnityEngine;

public class PipeEnterTrigger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer squidRenderer;
    [SerializeField] private SpriteRenderer[] interiorRenderers;
    private bool isSquidInside = false;
    private bool isInteriorVisible = false;

    private void Awake()
    {
        SetVisibility(false);
        isSquidInside = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Squid"))
        {
            SetVisibility(true);
            isSquidInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Squid"))
        {
            SetVisibility(false);
            isSquidInside = false;
        }
    }

    private void Update()
    {
        if (!PlayerController.Instance.isSquidActive && isSquidInside && isInteriorVisible) // On switch to Mariposa
        {
            squidRenderer.enabled = false;
            SetVisibility(false);
        }
        else if (PlayerController.Instance.isSquidActive && isSquidInside && !isInteriorVisible) // On switch to Squid
        {
            squidRenderer.enabled = true;
            SetVisibility(true);
        }
    }

    private void SetVisibility(bool isVisible)
    {
        isInteriorVisible = isVisible;
        foreach (SpriteRenderer renderer in interiorRenderers) 
            renderer.enabled = isVisible;
    }
}
