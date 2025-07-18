using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }
    [SerializeField] private Image _blackImage;
    [SerializeField] private float _fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _blackImage.enabled = true;
        _blackImage.color = new Color(0,0,0,1f);
    }

    private void Start()
    {   
        StartCoroutine(Fade(1f, 0f));
    }

    public void FadeOutAndDo(System.Action onComplete)
    {
        StartCoroutine(FadeCoroutine(onComplete));
    }

    public Coroutine FadeOut(System.Action onComplete = null)
    {
        return StartCoroutine(Fade(0f, 1f));
    }

    private IEnumerator FadeCoroutine(System.Action onComplete)
    {
        yield return Fade(0f, 1f);
        onComplete?.Invoke();
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / _fadeDuration);
            _blackImage.color = new Color(0,0,0,a);
            yield return null;
        }
        _blackImage.color = new Color(0,0,0,to);
    }
    public void FadeIn() => StartCoroutine(Fade(1f, 0f));
}