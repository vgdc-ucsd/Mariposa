using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UIButton = UnityEngine.UI.Button;

public class ButtonSoundApplicator : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(AddButtonSounds());
    }

    private IEnumerator AddButtonSounds()
    {
        yield return new WaitForEndOfFrame();

        UIButton[] existingButtons = FindObjectsByType<UIButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (UIButton button in existingButtons)
        {
            button.onClick.RemoveListener(PlayButtonSound);
            button.onClick.AddListener(PlayButtonSound);
        }
    }

    private void PlayButtonSound() => RuntimeManager.PlayOneShot(AudioEvents.SFX.button_click);
}
