using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public class EventDownloader : EditorWindow
{
    private string pastedConfig;
    private CSVConfig config;
    [MenuItem("Tools/Download Events")]
    public static void ShowWindow()
    {
        GetWindow<EventDownloader>("CSV Downloader");
    }

    private void OnEnable()
    {
        minSize = new Vector2(400, 300);
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheets Downloader Settings", EditorStyles.boldLabel);

        var newConfig = (CSVConfig)EditorGUILayout.ObjectField("Config File", config, typeof(CSVConfig), false);
        if (newConfig != config)
        {
            config = newConfig;
            UpdateConfigData();
        }
        if (GUILayout.Button("Download CSV Files"))
        {
            if (config != null)
            {
                DownloadCSVFiles();
            }
            else
            {
                Debug.LogError("Please assign a CSVConfig file.");
            }
        }
    }

    private void DownloadCSVFiles()
    {
        string urlTemplate = "https://script.google.com/macros/s/{0}/exec?sheetNameString={1}";
        Debug.Log("Download CSV Files!");

        foreach (var configEntry in config.dialogueConfigs)
        {
            foreach (var name in configEntry.sheetNames)
            {
                string url = string.Format(urlTemplate, configEntry.sheetID, name);
                string csvContent = DownloadFile(url);

                if (!string.IsNullOrEmpty(csvContent))
                {
                    string folderPath = Path.Combine("Assets/Resources/Events", configEntry.name);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    SaveToResources(csvContent, Path.Combine(folderPath, name + ".json")); 
                    Debug.Log($"Dialog for {name} downloaded and saved to {folderPath} successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to download the dialog file for {name}.");
                }
            }
        }
    }

    private string DownloadFile(string url)
    {
        try
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
        catch (System.Net.WebException ex)
        {
            Debug.LogError("Error downloading dialogue file: " + ex.Message);
            return null;
        }
    }

    private void UpdateConfigData()
    {
        pastedConfig = "";
        if (EditorGUIUtility.keyboardControl != 0) 
            GUI.FocusControl(null);
        foreach (var fig in config.dialogueConfigs)
        {
            pastedConfig += $"{fig.name}, {fig.sheetID}: {string.Join(", ", fig.sheetNames)}\n";
        }
    }

    private void SaveToResources(string csvContent, string filePath)
    {
        File.WriteAllText(filePath, csvContent);
        AssetDatabase.Refresh();
    }
}
#endif