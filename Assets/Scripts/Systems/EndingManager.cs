using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EndingType
{
    UNKNOWN,
    SILENT,
    NOT_SILENT,
    NOTHING_WRONG,
    FUTURE_LIE,
    FUTURE_TRUTH,
}

public class EndingManager : Singleton<EndingManager>
{
    public EndingType Ending = EndingType.UNKNOWN;
    public List<GameObject> DialogueTriggers;
    public float FadeDuration;

    void Start()
    {
        // TODO: for testing, remove before building
        FriendshipManager.Instance.SetScore(8);
        // TODO: replace with dialogue events for each branch that sets ending and active triggers
        if (FriendshipManager.Instance.CompareScore(7))
        {
            Ending = EndingType.NOT_SILENT;
            foreach (GameObject obj in DialogueTriggers) obj.SetActive(true);
        }
        else
        {
            Ending = EndingType.SILENT;
            foreach (GameObject obj in DialogueTriggers) obj.SetActive(false);
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // ENDING LOGIC

    public EndingType GetEnding() => Ending;
    // TODO: delete unused overload
    public void SetEnding(EndingType ending) => Ending = ending;
    public void SetEnding(int endingID) => Ending = (EndingType)endingID;

    public IEnumerator FadeSprites(List<SpriteRenderer> sprites, bool fadeIn)
    {
        float timer = 0;
        while (timer < FadeDuration)
        {
            timer += Time.deltaTime;
            foreach (SpriteRenderer sr in sprites)
            {
                sr.color = new(sr.color.r, sr.color.g, sr.color.b, fadeIn ? timer / FadeDuration : (FadeDuration - timer) / FadeDuration);
            }
            yield return new WaitForEndOfFrame();
        }

        foreach (SpriteRenderer sr in sprites)
        {
            sr.color = new(sr.color.r, sr.color.g, sr.color.b, fadeIn ? 1f : 0f);
        }
    }
}
