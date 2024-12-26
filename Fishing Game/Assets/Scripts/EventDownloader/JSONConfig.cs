using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JSONConfig", menuName = "ScriptableObjects/JSON Downloader Config")]
public class JSONConfig : ScriptableObject
{
    [System.Serializable]
    public class DialogueConfig
    {
        public string name;
        public string sheetID;
    }

    public List<DialogueConfig> dialogueConfigs;
}