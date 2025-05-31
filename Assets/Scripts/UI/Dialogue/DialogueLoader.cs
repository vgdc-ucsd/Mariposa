using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    [Header("The name of the yaml file to be loaded. Do not include the file extension.")]
    public List<string> DialogueNames;

    void Awake()
    {
        foreach (string dialogue in DialogueNames)
        {
            DialogueManager.Instance.LoadYaml(dialogue);
        }
    }
}
