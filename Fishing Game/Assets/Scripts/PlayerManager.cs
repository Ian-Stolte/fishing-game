using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public string name;
    public List<string> prereqs;
    public Dictionary<string, int> delayedPrereqs = new Dictionary<string, int>();
    
    //stats
    public int arts;
    public int smarts;
    public int heart;
    public int charm;
    public int money;

    private MapManager mapManager;


    public void Start()
    {
        mapManager = GameObject.Find("Map Manager").GetComponent<MapManager>();
    }

    public void Update()
    {
        if (delayedPrereqs.Count > 0)
        {
            List<string> keysToRemove = new List<string>();
            foreach (var kvp in delayedPrereqs)
            {
                if (kvp.Value == (mapManager.time + mapManager.day*3))
                {
                    keysToRemove.Add(kvp.Key);
                    prereqs.Add(kvp.Key);
                }
            }
            foreach (string str in keysToRemove)
                delayedPrereqs.Remove(str);
        }
    }


    public void ChangeStats(string stat, int amount)
    {
        if (stat == "Arts")
            arts += amount;
        else if (stat == "Smarts")
            smarts += amount;
        else if (stat == "Heart")
            heart += amount;
        else if (stat == "Charm")
            charm += amount;
        else if (stat == "Money")
            money += amount;
        else
            Debug.LogError("Stat " + stat + " not found!");
    }
}
