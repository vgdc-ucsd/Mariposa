using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : Singleton<EndingManager>
{
    private const int GOOD_ENDING_THRESHOLD = 16;
    [SerializeField] private List<GameObject> goodDialogueTriggers;
    [SerializeField] private List<GameObject> badDialogueTriggers;
    [SerializeField] private float fadeDuration;
    [SerializeField] private HometownCutscene cutsceneManager;

    private bool isGoodEnding;
    public bool IsGoodEnding
    {
        get { return isGoodEnding; }
        set
        {
            isGoodEnding = value;
            cutsceneManager.Animator.SetBool("IsGoodEnd", isGoodEnding);
        }
    }

    void Start()
    {
        // TODO: for testing, remove before building
        FriendshipManager.Instance.SetScore(GOOD_ENDING_THRESHOLD - 1);

        IsGoodEnding = FriendshipManager.Instance.CompareScore(GOOD_ENDING_THRESHOLD);
        foreach (GameObject obj in goodDialogueTriggers) obj.SetActive(IsGoodEnding);
        foreach (GameObject obj in badDialogueTriggers) obj.SetActive(!IsGoodEnding);
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

    private IEnumerator FadeIntoCutscene()
    {
		yield return FadeController.Instance.FadeOut();
    }

    public void EndIdleLoop()
    {
        Debug.Log("Ending idle loop");
        cutsceneManager.Animator.SetTrigger("EndIdleLoop");
    }
}
