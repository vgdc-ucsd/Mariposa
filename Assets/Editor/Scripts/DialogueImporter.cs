using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

enum DialogueFile
{
    TUTORIAL,
    DOWNTOWN,
    PIER,
    ROBOT,
    HOMETOWN,
    NPCS,
    FLAVOR_TEXT_AND_ITEMS
}

public class DialogueImporter : EditorWindow
{
    private DialogueFile dialogueFile;

    [MenuItem("Dialogue/Import Dialogue")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogueImporter), false, "Import Dialogue");
    }

    void OnGUI()
    {
        GUILayout.Label(".txt file", EditorStyles.boldLabel);
        dialogueFile = (DialogueFile)EditorGUILayout.EnumPopup("Dialogue File", dialogueFile);

        if (GUILayout.Button("Download & Import"))
        {
            DownloadDialogue(dialogueFile);
        }
    }

    private void DownloadDialogue(DialogueFile file)
    {
        string tabName = file.ToString().ToLower();
        WebClient client = new WebClient();
        client.DownloadFileCompleted += (sender, e) => ImportDialogue(tabName);
        client.DownloadFileAsync(GetDownloadURI(file), $"Assets/Dialogue/{tabName}.txt");
        Debug.Log($"Downloaded {tabName} successfully!");
    }

    private Uri GetDownloadURI(DialogueFile file)
    {
        string tab;
        switch (file)
        {
            case DialogueFile.TUTORIAL:
                tab = "t.0";
                break;
            case DialogueFile.DOWNTOWN:
                tab = "t.8u8ieea7akhd";
                break;
            case DialogueFile.PIER:
                tab = "t.2m9mur9dcbl0";
                break;
            case DialogueFile.ROBOT:
                tab = "t.1cv3585e0pa5";
                break;
            case DialogueFile.HOMETOWN:
                tab = "t.tpr04jqm3k88";
                break;
            case DialogueFile.NPCS:
                tab = "t.u7eieeoq9sbc";
                break;
            case DialogueFile.FLAVOR_TEXT_AND_ITEMS:
                tab = "t.k4tf1r7jez5h";
                break;
            default:
                tab = "";
                Debug.LogError($"No tab could be found for {file}");
                break;
        }

        string docBase = "https://docs.google.com/document/d/1s3rxKYcyUe-Ht71nyzD3D2npwpcDnx-PVLWIkDtFlDk/";
        return new Uri(docBase + "export?tab=" + tab + "&format=txt");
    }

    private string ParseLabel(string line)
    {
        string[] words = line.Split(" ");
        if (words.Length < 2) throw new Exception($"Error parsing file! The command {line} is missing a label!");
        return words[1];
    }

    private DialogueEventElement ParseEvent(string line)
    {
        string[] words = line.Split(" ");
        if (words.Length < 2) throw new Exception($"Error parsing file! The command {line} is missing a label!");
        DialogueEventElement eventElement = new DialogueEventElement();
        eventElement.eventName = words[1];
        if (words.Length == 3) eventElement.triggerAtEnd = words[2].ToLower() == "end";
        return eventElement;
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
        string icon = match.Groups[1].Value;
        return char.ToUpper(icon[0]) + icon.Substring(1);
    }

    private DialogueChoice ParseChoice(string line)
    {
        DialogueChoice choice = new DialogueChoice();
        string[] words = line.Split(" ");
        if (words.Length == 0) throw new Exception($"Error parsing file! The command {line} is missing a response!");
        choice.Response = words[0];

        if (words.Length == 2)
        {
            if (int.TryParse(words[1], out int friendship))
            {
                choice.Friendship = friendship;
            }
            else choice.LinkedDialogue = words[1];
        }
        else if (words.Length == 3)
        {
            choice.LinkedDialogue = words[1];
            choice.Friendship = int.Parse(words[2]);
        }

        return choice;
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
    void ImportDialogue(string tabName)
    {
        string filePath = $"Assets/Dialogue/{tabName}.txt";
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Could not fine file at {filePath}!");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        Dictionary<string, string> speakerPortrait = new Dictionary<string, string>();

        Dictionary<string, List<DialogueElement>> dd = new Dictionary<string, List<DialogueElement>>(); // dialogue dictionary
        DialogueElement element = new DialogueElement();
        string dialogueName = "";
        bool firstSpeaker = true;
        bool afterSpeaker = false;
        int afterChoice = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("#") || line.StartsWith("_")) continue; // comment or line break
            bool speaker = Regex.IsMatch(line, @"^[A-Z]+$");
            bool hasLineNext = false;
            if (i + 1 < lines.Length)
            {
                string next = lines[i + 1].Trim();
                hasLineNext = !string.IsNullOrWhiteSpace(next) && !next.StartsWith("#") && !next.StartsWith("!");
            }

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
                element.Events.Add(ParseEvent(line));
            }
            else if (Regex.IsMatch(line, @"^!(choice1|c1)\b")) // choice1
            {
                element.Choice1 = ParseChoice(line);
                afterChoice = 1;
            }
            else if (Regex.IsMatch(line, @"^!(choice2|c2)\b")) // choice2
            {
                element.Choice2 = ParseChoice(line);
                afterChoice = 2;
            }
            else if (Regex.IsMatch(line, @"^!(background|b)\b")) // background
            {
                element.Background = ParseLabel(line);
            }
            else if (Regex.IsMatch(line, @"^!(radio|r)\b")) // background
            {
                element.FromRadio = ParseLabel(line);
            }
            else if (speaker) // speaker
            {
                if (!firstSpeaker) dd[dialogueName].Add(element);
                else firstSpeaker = false;
                element = new DialogueElement();
                element.Speaker = ParseSpeaker(line);
                afterSpeaker = true;
            }
            else if (afterSpeaker && hasLineNext && Regex.IsMatch(line, @"^\([^)]+\)")) // icon
            {
                element.Icon = ParseIcon(line);
            }
            else if (afterChoice == 1)
            {
                element.Choice1.Response = line;
                afterChoice = 0;
            }
            else if (afterChoice == 2)
            {
                element.Choice2.Response = line;
                afterChoice = 0;
            }
            else // line
            {
                element.Line = ParseLine(line);
            }

            if (!speaker) afterSpeaker = false;
        }

        dd[dialogueName].Add(element);
        DialogueParser.Validate(dd);
        string assetName = tabName.Replace(" ", "_").Replace("\"", "") + ".yaml";
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
