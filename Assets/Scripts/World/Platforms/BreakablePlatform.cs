using System.Collections;
using UnityEditor;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    public enum PlatformState
    {
        Stable,
        Shaking,
        Broken,
    }

    public PlatformState state = PlatformState.Stable;

    public bool breakFromPlayer = false; // set to true if you want platform to shake/crumble after player stands on it for breakDelay time

    // ---------- variables for breaking from player ----------
    public SpriteRenderer spriteRenderer;    public float flashSpeed = 10f;
    public float breakDelay = 3f;
    public float flashTime = 2f;
    private Coroutine breakRoutine;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!breakFromPlayer || state != PlatformState.Stable) return;

        if (collider.gameObject == Player.ActivePlayer.gameObject)
        {
            breakRoutine = StartCoroutine(BeginBreakFromPlayer());
        }
    } 

    // uncomment if we want player to be able to reset a 'shaking' platform by moving off it
    // void OnTriggerExit2D(Collider2D collider)
    // {
    //     if (breakRoutine != null && collider.gameObject == Player.ActivePlayer.gameObject)
    //     {
    //         StopCoroutine(breakRoutine);
    //         breakRoutine = null;
    //         state = PlatformState.Stable;
    //     }
    // }

    IEnumerator BeginBreakFromPlayer()
    {
        yield return new WaitForSeconds(breakDelay);

        state = PlatformState.Shaking;
        Debug.Log("Platform is flashing...");

        float timer = 0f;
        Color originalColor = spriteRenderer.color;

        while (timer < flashTime)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;
        state = PlatformState.Broken;
        gameObject.SetActive(false);
    }

    public void BeenShot() {
        if (!breakFromPlayer && state != PlatformState.Broken) {
            state = PlatformState.Broken;
            gameObject.SetActive(false);
        }
    }
}