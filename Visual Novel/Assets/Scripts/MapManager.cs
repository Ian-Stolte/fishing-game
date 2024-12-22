using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    public Location[] locations;

    [SerializeField] private Transform eventSprites;
    [SerializeField] private TextMeshProUGUI eventTxt;
    [SerializeField] private Transform fader;
    
    [SerializeField] private GameObject timeTxt;
    private int time = -1;
    private int day = 1;
    private string[] timeStrings = new string[]{"Morning", "Afternoon", "Evening"};

    [SerializeField] private GameObject eveningOverlay;
    [SerializeField] private GameObject mapBG;
    [SerializeField] private GameObject locBG;
    [SerializeField] private Color[] mapColors;

    [SerializeField] private TextMeshProUGUI moneyQuest;
    [SerializeField] private TextMeshProUGUI fishQuest;

    [SerializeField] private Market market;
    private FishTracker fishTracker;
    private CharacterManager charManager;
    private EventManager eventManager;
    private PlayerManager player;


    void Start()
    {
        charManager = GameObject.Find("Character Manager").GetComponent<CharacterManager>();
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
        player = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
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
        locBG.GetComponent<EventPlayer>().SetupEvent(e.dialogue, n, time, sprites);
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

        //Quests
        moneyQuest.text = "Earn 100 sp. <b>(" + player.money + "/100)";
        if (player.money > 100)
        {
            moneyQuest.transform.GetChild(1).gameObject.SetActive(true);
            moneyQuest.color = new Color(200, 200, 200);
            //give player reward if first time
        }

        int caughtFish = 0;
        foreach (Fish f in fishTracker.fish)
        {
            if (f.totalCaught > 0)
                caughtFish++;
        }
        fishQuest.text = "Catch all 6 fish <b>(" + caughtFish + "/6)";
        if (caughtFish >= 6)
        {
            fishQuest.transform.GetChild(1).gameObject.SetActive(true);
            fishQuest.color = new Color(200, 200, 200);
            //give player reward if 1st time
        }
    }

    public IEnumerator ShowTimeTransition()
    {
        fader.GetComponent<Animator>().Play("FadeToDark");
        yield return new WaitForSeconds(0.5f);
        locBG.GetComponent<EventPlayer>().eventStarted = false;
        UpdateInfo();
        fader.GetChild(0).GetComponent<TextMeshProUGUI>().text = timeStrings[time];
        yield return new WaitForSeconds(2f);
        fader.GetComponent<Animator>().Play("FadeToLight");
        locBG.SetActive(false); 
    }
}


[System.Serializable]
public class Location
{
    public string name;
    public RectTransform iconPos;
    public List<string> charsHere;
}