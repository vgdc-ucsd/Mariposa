using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UnlockPipeEvent : MonoBehaviour
{
    [SerializeField] private ItemData pipeItemSO;
    [SerializeField] private GameObject pipeVisual;
    [SerializeField] private GameObject pipePickup;
    private float initialY;
    private const float n1 = 7.5625f;
    private const float d1 = 2.75f;

    public void UnlockPipe()
    {
        StartCoroutine(DropPipe());
    }

    IEnumerator DropPipe()
    {
        pipeVisual.SetActive(true);
        initialY = pipeVisual.transform.localPosition.y;
        float timer = 0;
        while (timer < 1f)
        {
            yield return null;
            timer += Time.deltaTime;
            pipeVisual.transform.localPosition = (1f - easeOutBounce(timer)) * initialY * Vector3.up;
        }
        pipeVisual.transform.localPosition = Vector3.zero;
        pipePickup.SetActive(true);
    }

    // function from https://easings.net/#easeOutBounce
    private float easeOutBounce(float x)
    {
        if (x < 1 / d1) {
            return n1 * x * x;
        } else if (x < 2 / d1) {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        } else if (x < 2.5 / d1) {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        } else {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }
}
