using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialogueImporter : EditorWindow
{
    private TextAsset txtFile;


    [MenuItem("Dialogue/Import Dialogue")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogueImporter), false, "Import Dialogue");
    }

    void OnGUI()
    {
        GUILayout.Label(".txt file", EditorStyles.boldLabel);
        txtFile = (TextAsset)EditorGUILayout.ObjectField(".txt file", txtFile, typeof(TextAsset), false);

        if (GUILayout.Button("Import"))
        {
            if (txtFile != null)
            {
                ImportDialogue(txtFile.text);
            }
            else
            {
                Debug.LogWarning("No file selected");
            }
        }
    }

    // Key
    // # - comment, skip this line
    void ImportDialogue(string text)
    {
        string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        Dictionary<string, string> speakerPortrait = new Dictionary<string, string>();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[0].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line[0].Equals("#")) continue; // comment

            //if (Regex.IsMatch(line, @""))

            //string assetName = newEvent.Title.Replace(" ", "_").Replace("\"", "");
            //AssetDatabase.CreateAsset(newEvent, $"Assets/ScriptableObjects/Events/{tag}/{assetName}.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Dialogue import complete!");
    }

}
