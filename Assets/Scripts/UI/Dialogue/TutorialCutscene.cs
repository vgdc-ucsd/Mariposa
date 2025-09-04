using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialSlideshowCutscene : MonoBehaviour
{
    [Header("Slides")]
    [SerializeField] private List<Sprite> slides;
    [SerializeField] private Image slideDisplay;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoRawImage;

    [Header("Settings")]
    [SerializeField] private float slideDuration = 3f;

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        Player.ActivePlayer.gameObject.SetActive(false);
        for (int i = 0; i < 3 && i < slides.Count; i++)
        {
            slideDisplay.sprite = slides[i];
            slideDisplay.gameObject.SetActive(true);
            yield return new WaitForSeconds(slideDuration);
        }
        Sprite lastSlide = slideDisplay.sprite;
        videoRawImage.gameObject.SetActive(true);
        if (videoPlayer.targetTexture != null)
        {
            videoPlayer.targetTexture.Release();
            videoPlayer.targetTexture.Create();
        }
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        slideDisplay.sprite = lastSlide;
        videoRawImage.gameObject.SetActive(true);
        videoPlayer.Play();
        slideDisplay.gameObject.SetActive(false);
        yield return new WaitUntil(() => !videoPlayer.isPlaying);
        videoRawImage.gameObject.SetActive(false);
        for (int i = 3; i < slides.Count; i++)
        {
            slideDisplay.sprite = slides[i];
            slideDisplay.gameObject.SetActive(true);
            yield return new WaitForSeconds(slideDuration);
        }
        slideDisplay.gameObject.SetActive(false);
        Player.ActivePlayer.gameObject.SetActive(true);
    }

}
