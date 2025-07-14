using System.Collections;
using UnityEngine;

public static class BasicAnimations
{
    public static IEnumerator Interpolate(System.Action onStart, System.Action<float> tween, System.Action onEnd, float duration)
    {
        float t = 0;
        float startTime = Time.time;
        onStart?.Invoke();

        float elapsedTime = Time.time - startTime; 
        while (elapsedTime < duration)
        {
            t = elapsedTime / duration;
            tween.Invoke(t);
            yield return null;
        }

        onEnd?.Invoke();
    }
}
