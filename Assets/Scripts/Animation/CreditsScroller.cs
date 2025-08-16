using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private float scrollTimeSeconds;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private TextMeshProUGUI unnamedNameText;

    void Start()
    {
        unnamedNameText.text = DataPersistenceManager.Instance.gameData.UnnamedName;
        StartCoroutine(CreditsCoroutine());
    }

    private IEnumerator CreditsCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
        yield return BasicAnimations.Interpolate
        (
            null,
            (t) => scrollRect.verticalNormalizedPosition = 1.0f - t,
            null,
            scrollTimeSeconds
        );
        GameManager.Instance.LoadScene(GameScene.MAIN_MENU);
    }
}
