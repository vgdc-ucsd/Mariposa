using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTester : MonoBehaviour
{
    public Transform ScrollContent;
    public GameObject DialogueTestButton;
    private Object[] dialogueData;

    void Start()
    {
        dialogueData = Resources.LoadAll("DialogueData", typeof(TextAsset));
        foreach (TextAsset dialogue in dialogueData)
        {
            DialogueManager.Instance.LoadYaml(dialogue.name);
        }

        Dictionary<string, List<DialogueElement>> dialogueDictionary = DialogueManager.Instance.GetDialogueDictionary();
        foreach ((string key, List<DialogueElement> elements) in dialogueDictionary)
        {
            GameObject testButton = Instantiate(DialogueTestButton);
            testButton.transform.SetParent(ScrollContent);

            UnityEngine.UI.Button button = testButton.GetComponent<UnityEngine.UI.Button>();
            TMP_Text tmp = testButton.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = key;
            button.onClick.AddListener(() =>
            {
                DialogueManager.Instance.PlayDialogue(key);
                button.image.color = Color.green;
            });
        }
    }
}
