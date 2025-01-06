using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public string name;
    public string[] pronouns;
    public List<string> prereqs;
    public Dictionary<string, int> delayedPrereqs = new Dictionary<string, int>();
    
    //stats
    public int arts;
    public int smarts;
    public int heart;
    public int charm;
    public int money;

    [SerializeField] private Color[] statColors;

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


    public void AddStats(string stat, int amount)
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
        {
            if (money - amount < 0)
                Debug.LogError("Not enough money to pay!");
            money += amount;
        }
        else
            Debug.LogError("Stat " + stat + " not found!");
    }

    public void MultiplyStats(string stat, float amount)
    {
        if (stat == "Arts")
            arts = (int)Mathf.Round(arts*amount);
        else if (stat == "Smarts")
            smarts = (int)Mathf.Round(smarts*amount);
        else if (stat == "Heart")
            heart = (int)Mathf.Round(heart*amount);
        else if (stat == "Charm")
            charm = (int)Mathf.Round(charm*amount);
        else if (stat == "Money")
            money = (int)Mathf.Round(money*amount);
        else
            Debug.LogError("Stat " + stat + " not found!");
    }

    public Color StatColor(string stat)
    {
        if (stat == "Arts")
            return statColors[0];
        else if (stat == "Smarts")
            return statColors[1];
        else if (stat == "Heart")
            return statColors[2];
        else if (stat == "Charm")
            return statColors[3];
        else if (stat == "Money")
            return statColors[4];
        else
        {
            Debug.LogError("Stat " + stat + " not found!");
            return new Color(255, 255, 255);
        }
    }

    public int StrToStat(string stat)
    {
        if (stat == "Arts")
            return arts;
        else if (stat == "Smarts")
            return smarts;
        else if (stat == "Heart")
            return heart;
        else if (stat == "Charm")
            return charm;
        else if (stat == "Money")
            return money;
        else
        {
            Debug.LogError("Stat " + stat + " not found!");
            return 0;
        }
    }
}
