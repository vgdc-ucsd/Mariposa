using System.Collections;
using UnityEngine;

public static class BasicAnimations
{
    public static IEnumerator Interpolate(System.Action onStart, System.Action<float> tween, System.Action onEnd, float duration)
    {
        float t = 0;
        float startTime = Time.time;
        onStart?.Invoke();

        float elapsedTime = 0; 
        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime; 
            t = elapsedTime / duration;
            tween.Invoke(t);
            yield return null;
        }

        onEnd?.Invoke();
    }

    // https://easings.net/
    public static float Smooth(float t) // Quadratic
    {
        t = t < 0.5f ? 2f * t * t : 1 - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        return t;
    }

    public static float EaseIn(float t)
    {
        t = t*t;
        return t;
    }

    public static float EaseOut(float t)
    {
        t = 1f - (1f - t) * (1f - t);
        return t;
    }
}
