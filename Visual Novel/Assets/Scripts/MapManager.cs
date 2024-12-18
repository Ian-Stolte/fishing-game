using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapManager : MonoBehaviour
{
    public Location[] locations;

    [SerializeField] private GameObject eventSummary;
    [SerializeField] private GameObject timeTxt;
    private int currentDay;
    private int currentLoop = 1;

    private CharacterManager charManager;
    private EventManager eventManager;
    private PlayerManager player;


    void Start()
    {
        charManager = GameObject.Find("Character Manager").GetComponent<CharacterManager>();
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
        player = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
        UpdateInfo();
    }

    public void SelectLocation(int n)
    {
        eventSummary.SetActive(true);
        eventSummary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You head to the " + locations[n].name + "...";
        Event e = eventManager.SelectEvent(n+1, locations[n].charsHere, currentDay, currentLoop);
        eventSummary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "You trigger: " + e.name;
        foreach (string req in e.prereqsGained)
            player.prereqs.Add(req);
        e.played = true;
    }

    public void UpdateInfo()
    {
        foreach (Location l in locations)
        {
            l.charsHere.Clear();
        }

        currentDay++;
        if (currentDay > 3)
        {
            currentDay = 1;
            currentLoop++;
        }
        timeTxt.GetComponent<TextMeshProUGUI>().text = "Day " + currentDay + ", Loop " + currentLoop;
        foreach (Character c in charManager.characters)
        {
            Vector3 loop = c.schedule[currentLoop-1];
            int loc = 0;
            if (currentDay == 1)
                loc = (int)loop.x;
            else if (currentDay == 2)
                loc = (int)loop.y;
            else if (currentDay == 3)
                loc = (int)loop.z;

            if (loc != 0)
            {
                locations[loc-1].charsHere.Add(c.name);
                c.icon.GetComponent<RectTransform>().anchoredPosition = locations[loc-1].iconPos.anchoredPosition - new Vector2(0, 50*(locations[loc-1].charsHere.Count-1));
                c.icon.SetActive(true);
            }
            else
            {
                c.icon.SetActive(false);
            }
        }
    }
}


[System.Serializable]
public class Location
{
    public string name;
    public RectTransform iconPos;
    //public CharacterPct[] characterPcts;
    public List<string> charsHere;
}

[System.Serializable]
public class CharacterPct
{
    public float orange;
    public float green;
    public float purple;
}