using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    public Location[] locations;

    [SerializeField] private GameObject eventSummary;
    [SerializeField] private Transform eventSprites;
    [SerializeField] private TextMeshProUGUI eventTxt;
    
    [SerializeField] private GameObject timeTxt;
    private int time = -1;
    private int day = 1;
    private string[] timeStrings = new string[]{"Morning", "Afternoon", "Evening"};

    [SerializeField] private GameObject eveningOverlay;
    [SerializeField] private GameObject mapBG;
    [SerializeField] private GameObject locBG;
    [SerializeField] private Color[] mapColors;

    [SerializeField] private Market market;
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
        Event e = eventManager.SelectEvent(n+1, locations[n].charsHere, time, day);
        
        locBG.SetActive(true);
        locBG.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = locations[n].name;
        locBG.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = e.name;
        List<GameObject> sprites = new List<GameObject>();
        foreach (string name in e.chars)
        {
            foreach (Character c in charManager.characters)
            {
                if (name == c.name)
                {
                    sprites.Add(c.sprite);
                }
            }
        }
        foreach (Transform child in eventSprites)
            child.gameObject.SetActive(false);
        foreach (string req in e.prereqsGained)
            player.prereqs.Add(req);
        e.played = true;
        locBG.GetComponent<EventPlayer>().SetupEvent(e.dialogue, n, time, sprites, (n!=0));
    }

    public void UpdateInfo()
    {
        //update day & time
        time++;
        if (time > 2)
        {
            time = 0;
            day++;
            market.visitedToday = false;
        }
        timeTxt.GetComponent<TextMeshProUGUI>().text =  "Day " + day + " - " + timeStrings[time];
        mapBG.GetComponent<Image>().color = mapColors[time];
        locBG.GetComponent<Image>().color = mapColors[time];
        eveningOverlay.SetActive(time==2);

        //move character icons
        foreach (Location l in locations)
        {
            l.charsHere.Clear();
        }
        foreach (Character c in charManager.characters)
        {
            Vector3 loop = c.schedule[(day-1)%c.schedule.Length];
            int loc = 0;
            if (time == 0)
                loc = (int)loop.x;
            else if (time == 1)
                loc = (int)loop.y;
            else if (time == 2)
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
    public List<string> charsHere;
}