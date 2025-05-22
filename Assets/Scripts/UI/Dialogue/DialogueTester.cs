using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    private Object[] dialogueData;

    void Start()
    {
        dialogueData = Resources.LoadAll("DialogueData", typeof(TextAsset));
        foreach (TextAsset dialogue in dialogueData)
        {
            //DialogueManager.Instance.LoadYaml(dialogue.name);
        }

        TextAsset test = (TextAsset)dialogueData[0];
        DialogueManager.Instance.LoadYaml(test.name);
        DialogueManager.Instance.PlayDialogue("Opening1");
    }
}
