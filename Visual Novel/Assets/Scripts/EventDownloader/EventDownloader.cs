using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public class CSVDownloaderEditor : EditorWindow
{
    private string pastedConfig;
    private CSVConfig config;
    [MenuItem("Tools/Download Events")]
    public static void ShowWindow()
    {
        GetWindow<CSVDownloaderEditor>("CSV Downloader");
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

    /*private void ParseAndSaveConfig()
    {
        string[] entries = pastedConfig.Split('\n');
        List<CSVDownloaderConfig.DialogConfig> parsedConfigs = new List<CSVDownloaderConfig.DialogConfig>();

        foreach (string entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry)) continue;

            string[] parts = entry.Split(new[] { ',' }, 2); 
            if (parts.Length == 2)
            {
                string name = parts[0].Trim(); 
                string[] sheetParts = parts[1].Split(':');

                if (sheetParts.Length == 2)
                {
                    string sheetID = sheetParts[0].Trim(); 
                    string[] sheetNames = sheetParts[1].Split(',');

                    DialogueConfig dialogConfig = new DialogConfig
                    {
                        name = name,
                        sheetID = sheetID,
                        sheetNames = new List<string>()
                    };

                    foreach (string sheetName in sheetNames)
                    {
                        dialogConfig.sheetNames.Add(sheetName.Trim());
                    }

                    parsedConfigs.Add(dialogConfig);
                }
                else
                {
                    Debug.LogError("Invalid format. Each entry must be in the format 'name, sheetID: sheetName1, sheetName2'.");
                }
            }
            else
            {
                Debug.LogError("Invalid format. Each entry must contain a name and sheet configuration.");
            }
        }

        if (parsedConfigs.Count > 0)
        {
            CSVDownloaderConfig newConfig = CreateInstance<CSVDownloaderConfig>();
            newConfig.dialogConfigs = parsedConfigs;
            if (config != null)
            {
                config.dialogConfigs = newConfig.dialogConfigs;
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Configuration updated successfully!");
                return;
            }
            string path = EditorUtility.SaveFilePanelInProject("Save Config", "CSVDownloaderConfig", "asset", "Please enter a file name to save the config");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newConfig, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Configuration saved successfully!");
            }
        }
        else
        {
            Debug.LogError("No valid configuration parsed.");
        }
    }*/

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
                    string folderPath = Path.Combine("Assets/Events", configEntry.name);
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
            Debug.LogError("Error downloading dialog file: " + ex.Message);
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