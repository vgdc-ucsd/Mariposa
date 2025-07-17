using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : Singleton<EndingManager>
{
    private const int GOOD_ENDING_THRESHOLD = 16;
    [SerializeField] private List<GameObject> dialogueTriggers;
    [SerializeField] private float fadeDuration;

    void Start()
    {
        // TODO: for testing, remove before building
        FriendshipManager.Instance.SetScore(8);

        bool shiftTriggersActive = FriendshipManager.Instance.CompareScore(GOOD_ENDING_THRESHOLD);
        foreach (GameObject obj in dialogueTriggers) obj.SetActive(shiftTriggersActive);
    }

    public IEnumerator FadeSprites(List<SpriteRenderer> sprites, bool fadeIn)
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            foreach (SpriteRenderer sr in sprites)
            {
                sr.color = new(sr.color.r, sr.color.g, sr.color.b, fadeIn ? timer / fadeDuration : (fadeDuration - timer) / fadeDuration);
            }
            yield return new WaitForEndOfFrame();
        }

        foreach (SpriteRenderer sr in sprites)
        {
            sr.color = new(sr.color.r, sr.color.g, sr.color.b, fadeIn ? 1f : 0f);
        }
    }
}
