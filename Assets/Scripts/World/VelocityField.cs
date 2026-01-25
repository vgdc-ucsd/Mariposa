using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class VelocityField : MonoBehaviour
{
    [Tooltip("The acceleration to apply to the Bee when it stays in the field")]
    public float blowForce = 70f;
    [SerializeField] private BoxCollider2D blowField;
    [SerializeField] private LayerMask barrierLayerMask;
    [SerializeField] private ParticleSystem windParticles;
    private Vector2 fieldMaxSize;
    private Vector2 blowDirection = Vector2.zero;

    private BeeMovement bee;

    public bool inactiveOnStart = false;

    private EventInstance fanInstance;

    private EventReference fanReference;
    private void Awake()
    {
        fanReference = AudioEvents.SFX.fan_vent;
        fanInstance = RuntimeManager.CreateInstance(fanReference);
        RuntimeManager.AttachInstanceToGameObject(fanInstance,gameObject);
        fieldMaxSize = blowField.transform.localScale;
        RecomputeFieldCollider();
        if (inactiveOnStart)
        {
            windParticles.Stop();
            windParticles.Clear();
            gameObject.SetActive(false);
        }
        else
        {
            fanInstance.start();
        }
    }

    private void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        if (bee != null)
        {
            bee.Velocity += blowForce * fdt * blowDirection;
            bee.Velocity = Vector2.ClampMagnitude(bee.Velocity, bee.TerminalVelocity);
        }
    }

    private const float CAST_OFFSET = 0.1f;
    [ContextMenu("Recompute Field Collider")]
    public void RecomputeFieldCollider()
    {
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        blowDirection = new(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 castOrigin = (Vector2)transform.parent.position + CAST_OFFSET * blowDirection;
        float castLength = fieldMaxSize.x - CAST_OFFSET;
        RaycastHit2D hit = Physics2D.Raycast(castOrigin, blowDirection, castLength, barrierLayerMask);

        float newLength = hit
            ? hit.distance + CAST_OFFSET
            : fieldMaxSize.x;
        var main = windParticles.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(newLength / windParticles.main.startSpeed.constant, newLength / windParticles.main.startSpeed.constant);
        blowField.transform.localScale = new(newLength, fieldMaxSize.y);
        blowField.transform.localPosition = new Vector2(newLength / 2f, 0f);
        // Debug.Log(newLength); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bee")) return;

        bee = collision.GetComponent<BeeMovement>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bee")) return;

        bee = null;
    }

    public void OnFieldToggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf)
        {
            windParticles.gameObject.SetActive(true);
            windParticles.Play();
            if(!AudioManager.IsPlaying(fanInstance)) fanInstance.start();
            Debug.Log("Fan sfx: " + fanInstance.isValid());
        }
        else
        {
            windParticles.Stop();
            fanInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Debug.Log("Fan sfx stop playing");
        }
    }
}
