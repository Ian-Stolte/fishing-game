using System.Linq;
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
    [SerializeField] private GameObject calendar;
    [SerializeField] private GameObject calendarEvent;
    [SerializeField] private GameObject calendarUpdate;
    [SerializeField] private GameObject calendarArrow;
    [SerializeField] private GameObject currentTime;
    private bool calendarOpen;
    private bool timeTransition;
    [SerializeField] private Color[] calendarColors;

     [SerializeField] private GameObject inventory;
    
    [SerializeField] private GameObject timeTxt;
    public int time = -1;
    public int day = 1;
    private string[] timeStrings = new string[]{"Morning", "Afternoon", "Evening"};

    public Transform gardenPlants;
    [SerializeField] private GameObject eveningOverlay;
    [SerializeField] private GameObject mapBG;
    [SerializeField] private GameObject locBG;
    [SerializeField] private Color[] mapColors;

    [SerializeField] private TextMeshProUGUI moneyQuest;
    [SerializeField] private TextMeshProUGUI fishQuest;

    [SerializeField] private Market market;
    [SerializeField] private Garden garden;
    private FishTracker fishTracker;
    private FoodTracker foodTracker;
    private CharacterManager charManager;
    private EventManager eventManager;
    private PlayerManager player;
    private AudioManager audioManager;


    void Start()
    {
        charManager = GameObject.Find("Character Manager").GetComponent<CharacterManager>();
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
        player = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
        foodTracker = GameObject.Find("Food Tracker").GetComponent<FoodTracker>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        UpdateInfo();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !timeTransition)
        {
            calendarOpen = !calendarOpen;
            StopCoroutine("OpenCalendar");
            StartCoroutine("OpenCalendar");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventory.activeSelf)
                inventory.SetActive(false);
            else
                ShowInventory();   
        }
    }

    private IEnumerator OpenCalendar()
    {
        if (!calendarOpen)
        {
            while (calendar.GetComponent<RectTransform>().anchoredPosition.x > -1925)
            {
                float speed = (-1f/30000) * Mathf.Pow(calendar.GetComponent<RectTransform>().anchoredPosition.x + 1000, 2) + 40;
                calendar.GetComponent<RectTransform>().anchoredPosition -= new Vector2(speed, 0);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (calendar.GetComponent<RectTransform>().anchoredPosition.x < 0)
            {
                float speed = (-1f/30000) * Mathf.Pow(calendar.GetComponent<RectTransform>().anchoredPosition.x + 1000, 2) + 40;
                calendar.GetComponent<RectTransform>().anchoredPosition += new Vector2(speed, 0);
                yield return new WaitForSeconds(0.01f);
            }
            calendar.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    private void ShowInventory()
    {
        //set up fish
        foreach (Transform child in inventory.transform.GetChild(1)) //food
        {
            Food f = foodTracker.food.FirstOrDefault(f => f.name == child.name);
            if (f != null)
            {
                child.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + f.quantity;
                child.GetChild(2).gameObject.SetActive(f.quantity == 0);
            }
        }
        foreach (Transform child in inventory.transform.GetChild(2)) //seeds
        {
            Seed s = garden.seeds.FirstOrDefault(s => s.name == child.name.Substring(0, child.name.Length-6));
            if (s != null)
            {
                child.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + s.quantity;
                child.GetChild(2).gameObject.SetActive(s.quantity == 0);
            }
        }
        inventory.SetActive(true);
    }


    public void SelectLocation(int n)
    {
        Event e = eventManager.SelectEvent(n+1, locations[n].charsHere, time, day);
        
        locBG.SetActive(true);
        foreach (Transform child in eventSprites)
            child.gameObject.SetActive(false);
        locBG.GetComponent<EventPlayer>().SetupEvent(e, n, time);
    }


    public void UpdateInfo()
    {
        //update day & time
        if (time != -1)
        {
            foreach (Transform child in calendar.transform.GetChild(day))
            {
                if (child.GetComponent<Image>() != null)
                {
                    if (child.GetComponent<Image>().color == calendarColors[time])
                    {
                        child.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6f, 0.6f, 0.6f);
                        child.GetComponent<Image>().color = new Color(child.GetComponent<Image>().color.r, child.GetComponent<Image>().color.g, child.GetComponent<Image>().color.b, 0.6f);
                    }
                }    
            }
        }
        time++;
        if (time > 2)
        {
            time = 0;
            day++;
            market.visitedToday = false;
        }
        if (time == 1)
            currentTime.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -10);
        else if (time == 2)
            currentTime.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -110);
        timeTxt.GetComponent<TextMeshProUGUI>().text =  "Day " + day + " - " + timeStrings[time];
        mapBG.GetComponent<Image>().color = mapColors[time];
        locBG.GetComponent<Image>().color = mapColors[time];
        eveningOverlay.SetActive(time==2);
        
        //Garden
        foreach (Transform child in gardenPlants)
        {
            Plant p = child.GetComponent<Plant>();
            if (p != null) //if not an empty box
            {
                p.time += 1;
                child.GetChild(2).GetChild(1).GetComponent<Image>().fillAmount = (p.time*1.0f)/p.totalTime;
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


    public string ReturnModdedTime(int timeChange)
    {
        int newTime = time + timeChange;
        int dayDiff = newTime/3;
        newTime = newTime%3;

        if (dayDiff == 0)
            return ("this " + timeStrings[newTime].ToLower());
        else if (dayDiff == -1)
            return ("yesterday " + timeStrings[newTime].ToLower());
        else if (dayDiff == 1)
            return ("tomorrow " + timeStrings[newTime].ToLower());
        else
        {
            int newDay = day + dayDiff;
            string suffix = "st";
            if (newDay%10 == 1)
                suffix = "st";
            else if (newDay%10 == 2)
                suffix = "nd";
            return ("on the " + timeStrings[newTime].ToLower() + " of the " + (day + dayDiff) + suffix);
        }
    }

    
    public string TimeString()
    {
        return timeStrings[time].ToLower();
    }


    public IEnumerator ShowTimeTransition()
    {
        timeTransition = true;
        fader.GetChild(0).gameObject.SetActive(time != 2);
        fader.GetComponent<Animator>().Play("FadeToDark");
        if (time == 2)
            audioManager.CalendarMusic();
        yield return new WaitForSeconds(0.5f);
        locBG.GetComponent<EventPlayer>().eventStarted = false;
        UpdateInfo();
        fader.GetChild(0).GetComponent<TextMeshProUGUI>().text = timeStrings[time];
        if (time == 0)
        {
            calendarOpen = true;
            StartCoroutine("OpenCalendar");
            yield return new WaitForSeconds(1.8f);
            while (currentTime.GetComponent<RectTransform>().anchoredPosition.y > -150)
            {
                currentTime.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 1.2f);
                yield return new WaitForSeconds(0.02f);
            }
            currentTime.transform.SetParent(calendar.transform.GetChild(day));
            currentTime.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            while (currentTime.GetComponent<RectTransform>().anchoredPosition.y > 75)
            {
                currentTime.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 1.2f);
                yield return new WaitForSeconds(0.02f);
            }
            calendarArrow.SetActive(true);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            calendarArrow.SetActive(false);
            calendarOpen = false;
            StartCoroutine("OpenCalendar");
            StartCoroutine(audioManager.StartFade("Sweet Dreams", 1, 0));
            yield return new WaitForSeconds(0.5f);
            audioManager.NewSong();
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        fader.GetComponent<Animator>().Play("FadeToLight");
        locBG.SetActive(false);
        timeTransition = false;
    }


    public void AddToCalendar(string name, int addedTime)
    {
        int newTime = time + addedTime;
        int newDay = day + newTime/3;
        newTime = newTime%3;
        GameObject calEvent = Instantiate(calendarEvent, Vector3.zero, Quaternion.identity, calendar.transform.GetChild(newDay));
        TextMeshProUGUI txt = calEvent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txt.text = name;
        if (txt.preferredHeight > 60)
        {
            txt.fontSize = 32;
        }
        calEvent.GetComponent<RectTransform>().sizeDelta = new Vector2(calEvent.GetComponent<RectTransform>().sizeDelta.x, txt.preferredHeight+20);
        if (newTime==0)
            calEvent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 90-calEvent.GetComponent<RectTransform>().sizeDelta.y/2);
        else if (newTime==1)
            calEvent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20);
        else if (newTime==2)
            calEvent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -130+calEvent.GetComponent<RectTransform>().sizeDelta.y/2);
        
        calEvent.GetComponent<Image>().color = calendarColors[newTime];
        calendarUpdate.GetComponent<Animator>().Play("CalendarUpdate");
        Debug.Log("Adding " + name + " to calendar at day=" + newDay + ", time=" + newTime);
        currentTime.transform.SetSiblingIndex(currentTime.transform.parent.childCount-1);
    }
}


[System.Serializable]
public class Location
{
    public string name;
    public RectTransform iconPos;
    public List<string> charsHere;
}