using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class KeycardRejectEvent : DialogueEvent
{
    [SerializeField] private Transform keycard;
    [SerializeField] private SplineContainer path;
    private const string DIALOGUE = "turnstile_keycard_rejected";


    private IEnumerator KeycardAnimation()
    {
        float3 pos;
        float3 tangent;
        float3 up;
        keycard.gameObject.SetActive(true);

        yield return BasicAnimations.Interpolate
        (
            null,
            (float t) =>
            {
                path.Spline.Evaluate<Spline>(t, out pos, out tangent, out up);
                keycard.position = pos;
            },
            null,
            0.5f
        );

        DialogueManager.Instance.PlayDialogue(DIALOGUE);
    }

    public override void Trigger()
    {
        StartCoroutine(KeycardAnimation());
    }
}
