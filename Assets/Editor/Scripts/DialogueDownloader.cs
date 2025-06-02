using UnityEngine;
using UnityEditor;
using System.Net;

public class DialogueDownloader : EditorWindow
{
    private string tabName = "";

    [MenuItem("Dialogue/Download Dialogue")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogueDownloader), false, "Download Dialogue");
    }

    void OnGUI()
    {
        GUILayout.Label("Tab name", EditorStyles.boldLabel);
        tabName = EditorGUILayout.TextField("Tab name: ", tabName);

        if (GUILayout.Button("Download"))
        {
            if (!string.IsNullOrEmpty(tabName))
            {
                DownloadDialogue(tabName);
            }
            else
            {
                Debug.LogWarning("Please enter a tab to download");
            }
        }
    }

    private void DownloadDialogue(string tab)
    {
        WebClient client = new WebClient();
        client.DownloadFile("https://docs.google.com/document/d/14jt0HTbukhN3dQlm43BTEvQhlbZ7M25Kqu1Ud9NwCiA/export?tab=t.8u8ieea7akhd&format=txt", $"Assets/Dialogue/{tab}.txt");
        Debug.Log($"Downloaded {tab} successfully!");
    }
}
