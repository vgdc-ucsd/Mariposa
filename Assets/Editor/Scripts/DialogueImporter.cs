using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

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
                ImportDialogue(txtFile);
            }
            else
            {
                Debug.LogWarning("No file selected");
            }
        }
    }

    private string ParseLabel(string line)
    {
        string[] words = line.Split(" ");
        if (words.Length < 2) throw new Exception($"Error parsing file! The command {line} is missing a label!");
        return words[1];
    }

    private string ParseSpeaker(string line)
    {
        string speaker = line.ToLower();
        return char.ToUpper(speaker[0]) + speaker.Substring(1);      
    }

    private string ParseIcon(string line)
    {
        string innerTextPattern = @"^\(([^)]+)\)";
        Match match = Regex.Match(line, innerTextPattern);
        return match.Groups[1].Value;
    }

    private string ParseLine(string line)
    {
        // TODO quotes
        return line;
    }

    // Key
    // # - comment
    // CAPS - speaker
    // (text) - icon
    // !name/!n - name of the conversation
    // !audio/!a - sound effect
    // !event/!e - arbitrary code
    // default text - dialogue line
    void ImportDialogue(TextAsset dialogue)
    {
        string[] lines = dialogue.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        Dictionary<string, string> speakerPortrait = new Dictionary<string, string>();
        
        Dictionary<string, List<DialogueElement>> dd = new Dictionary<string, List<DialogueElement>>(); // dialogue dictionary
        DialogueElement element = new DialogueElement();
        string dialogueName = "";
        bool firstSpeaker = true;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("#") || line.StartsWith("_")) continue; // comment or line break

            if (Regex.IsMatch(line, @"^!(name|n)\b")) // dialogue name
            {
                if (dialogueName != "") dd[dialogueName].Add(element);

                dialogueName = ParseLabel(line);
                if (dd.ContainsKey(dialogueName))
                {
                    throw new DialogueException(dialogueName, $"Multiple dialogues share the same name \"{dialogueName}\"!");
                }

                dd[dialogueName] = new List<DialogueElement>();
                element = new DialogueElement();
                firstSpeaker = true;
            }
            else if (Regex.IsMatch(line, @"^!(audio|a)\b")) // audio
            {
                element.Sounds.Add(ParseLabel(line));
            }
            else if (Regex.IsMatch(line, @"^!(event|e)\b")) // event
            {
                element.Events.Add(ParseLabel(line));
            }
            else if (Regex.IsMatch(line, @"^[A-Z]+$")) // speaker
            {
                if (!firstSpeaker) dd[dialogueName].Add(element);
                else firstSpeaker = false;
                element = new DialogueElement();
                element.Speaker = ParseSpeaker(line);
            }
            else if (Regex.IsMatch(line, @"^\([^)]+\)")) // icon
            {
                element.Icon = ParseIcon(line);
            }
            else // line
            {
                element.Line = ParseLine(line);
            }
        }

        dd[dialogueName].Add(element);
        //DialogueParser.Validate(dd);
        string assetName = dialogue.name.Replace(" ", "_").Replace("\"", "") + ".yaml";
        string yaml = DialogueParser.ToYaml(dd);
        string assetPath = Path.Combine("Resources/DialogueData", assetName);
        string fullPath = Path.Combine(Application.dataPath, assetPath);
        File.WriteAllText(fullPath, yaml);

        AssetDatabase.ImportAsset(assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Successfully created {assetName}");
    }
}
