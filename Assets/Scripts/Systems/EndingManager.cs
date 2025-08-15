using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public bool IsCutsceneActive { get; private set; }

    void Start()
    {
        // TODO: for testing, remove before building
        FriendshipManager.Instance.SetScore(GOOD_ENDING_THRESHOLD);

        IsCutsceneActive = false;
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

    public IEnumerator FadeImages(List<Image> images, bool fadeIn)
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            foreach (Image img in images)
            {
                img.color = new(img.color.r, img.color.g, img.color.b, fadeIn ? timer / fadeDuration : (fadeDuration - timer) / fadeDuration);
            }
            yield return new WaitForEndOfFrame();
        }

        foreach (Image img in images)
        {
            img.color = new(img.color.r, img.color.g, img.color.b, fadeIn ? 1f : 0f);
        }
    }

    public void PlayCutscene()
    {
        IsCutsceneActive = true;
        cutsceneManager.Animator.Play("EnterHouse");
    }

    public void AdvanceCutscene()
    {
        cutsceneManager.Animator.SetTrigger("AdvanceCutscene");
    }

    public void EndCutscene()
    {
        // Calling these causes screen to flicker back to hometown level before returning to menu
        // They should be unnecessary unless there are issues with persistent state when returning to hometown after finishing
        // DialogueManager.Instance.EndCutscene();
        // IsCutsceneActive = false;

        // TODO: go to credits
        GameManager.Instance.LoadScene(GameScene.MAIN_MENU);
    }
}
