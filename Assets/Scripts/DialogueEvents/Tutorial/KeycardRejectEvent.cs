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
        Vector3 origin = path.transform.position;
        float3 pos;
        float3 tangent;
        float3 up;
        keycard.gameObject.SetActive(true);

        Collider2D collider = keycard.GetComponentInChildren<Collider2D>();
        collider.enabled = false;

        yield return BasicAnimations.Interpolate
        (
            null,
            (float t) =>
            {
                path.Spline.Evaluate<Spline>(t, out pos, out tangent, out up);
                keycard.position = origin + new Vector3(pos.x, pos.y, pos.z);
            },
            null,
            0.5f
        );

        collider.enabled = true;
        DialogueManager.Instance.PlayDialogue(DIALOGUE);
    }

    public override void Trigger()
    {
        StartCoroutine(KeycardAnimation());
    }
}
