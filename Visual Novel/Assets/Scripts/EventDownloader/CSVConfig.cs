using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CSVDownloaderConfig", menuName = "ScriptableObjects/CSV Downloader Config")]
public class CSVConfig : ScriptableObject
{
    [System.Serializable]
    public class DialogueConfig
    {
        public string name;
        public string sheetID;
        public List<string> sheetNames;
    }

    public List<DialogueConfig> dialogueConfigs;
}