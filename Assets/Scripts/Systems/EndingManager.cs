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
    public List<SpriteRenderer> MariposaTrees;
    public List<SpriteRenderer> MariposaAutomatons;
    public float FadeDuration;

    void Start()
    {
        if (FriendshipManager.Instance.IsGoodScore())
        {
            Ending = EndingType.SILENT;
        }
        else
        {
            Ending = EndingType.NOT_SILENT;
            // TODO: play dialogue, implement music/art switching
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

    // VISUAL FADING
    
    public void ShowTrees()
    {
        StartCoroutine(FadeSprites(MariposaTrees));
    }

    public void HideTrees()
    {
        StartCoroutine(FadeSprites(MariposaTrees));
    }

    public void ShowAutomatons()
    {
        StartCoroutine(FadeSprites(MariposaAutomatons));
    }

    public void HideAutomatons()
    {
        StartCoroutine(FadeSprites(MariposaAutomatons));
    }

    IEnumerator FadeSprites(List<SpriteRenderer> sprites)
    {
        float timer = 0;
        while (timer < FadeDuration)
        {
            timer += Time.deltaTime;
            foreach (SpriteRenderer sr in sprites)
            {
                sr.color = new(sr.color.r, sr.color.g, sr.color.b, timer / FadeDuration);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
