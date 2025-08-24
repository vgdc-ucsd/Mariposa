using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class UnlockPipeEvent : DialogueEvent
{
    [SerializeField] private ItemData pipeItemSO;
    [SerializeField] private GameObject pipeVisual;
    [SerializeField] private GameObject pipePickup;
    [SerializeField] private PipeEnterTrigger pipes;
    [SerializeField] private CinemachineCamera pipeCamera;

    private float initialY;
    private const float DROP_TIME = 2.0f;
    private const float n1 = 7.5625f;
    private const float d1 = 2.75f;

    IEnumerator DropPipe()
    {
        pipeCamera.gameObject.SetActive(true);
        PlayerController.Instance.SetMovementLock(true);
        pipes.SetVisibility(true);
        pipeVisual.SetActive(true);
        initialY = pipeVisual.transform.localPosition.y;
        float timer = 0;
        while (timer < DROP_TIME)
        {
            yield return null;
            timer += Time.deltaTime;
            pipeVisual.transform.localPosition = (1f - easeOutBounce(timer / DROP_TIME)) * initialY * Vector3.up;
        }
        pipeVisual.transform.localPosition = Vector3.zero;
        pipePickup.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        pipes.SetVisibility(false);
        PlayerController.Instance.SetMovementLock(false);
        pipeCamera.gameObject.SetActive(false);
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

    public override void Trigger()
    {
        StartCoroutine(DropPipe());
    }
}
