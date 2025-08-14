using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TextInputDisplay : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI characterLimit;

    void Update()
    {
        characterLimit.text = $"{inputField.text.Length}/{inputField.characterLimit}";
    }

    public void Open()
    {
        PlayerController.Instance.SetMovementLock(true);
        DebugManager.Instance.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        DialogueManager.Instance.PlayDialogue("tell_her_your_name_after_naming");
        PlayerController.Instance.SetMovementLock(false);
        DebugManager.Instance.gameObject.SetActive(true);
    }

    public void SubmitName()
    {
        DataPersistenceManager.Instance.gameData.UnnamedName = inputField.text;
        Close();
    }
}
