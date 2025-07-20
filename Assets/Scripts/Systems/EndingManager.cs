using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : Singleton<EndingManager>
{
    public enum Ending
    {
        SilicaHeart,
        PlumVinegar,
        OrangeSunset,
        GoodnightMariposa,
    }

    private const int GOOD_ENDING_THRESHOLD = 16;
    [SerializeField] private List<GameObject> goodDialogueTriggers;
    [SerializeField] private List<GameObject> badDialogueTriggers;
    [SerializeField] private float fadeDuration;
    [SerializeField] private HometownCutscene cutsceneManager;

    private Ending currentEnding;
    public Ending CurrentEnding
    {
        get { return currentEnding; }
        set
        {
            currentEnding = value;
            cutsceneManager.Animator.SetBool(currentEnding.ToString(), true);
        }
    }

    void Start()
    {
        // TODO: for testing, remove before building
        FriendshipManager.Instance.SetScore(GOOD_ENDING_THRESHOLD);

        bool isGoodEnding = FriendshipManager.Instance.CompareScore(GOOD_ENDING_THRESHOLD);
        foreach (GameObject obj in goodDialogueTriggers) obj.SetActive(isGoodEnding);
        foreach (GameObject obj in badDialogueTriggers) obj.SetActive(!isGoodEnding);
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

    public void PlayCutscene()
    {
        // cutsceneManager.Animator.Play("FadeIn");
        cutsceneManager.Animator.Play("EnterHouse");
    }

    public void EndIdleLoop()
    {
        Debug.Log("Ending idle loop");
        cutsceneManager.Animator.SetTrigger("EndIdleLoop");
    }
}
