using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventManager : MonoBehaviour
{
    public Location[] locations;

    [SerializeField] private GameObject eventSummary;
    [SerializeField] private GameObject timeTxt;
    private int currentDay;
    private int currentLoop = 1;

    [SerializeField] private CharacterManager charManager;




    void Start()
    {
        UpdateInfo();
    }

    public void SelectLocation(int n)
    {
        eventSummary.SetActive(true);
        eventSummary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You head to the " + locations[n].name + "...";
        string chars = "You encounter: ";
        bool anyChars = false;
        foreach(string name in locations[n].charsHere)
        {
            chars += name + ", ";
            anyChars = true;
        }
        if (anyChars)
            eventSummary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = chars.Substring(0, chars.Length-2);
        else
            eventSummary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "You have a great time!";
        
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
                //c.icon.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                //Debug.Log(c.name + " -> " + locations[loc-1].name);
                //Debug.Log(locations[loc-1].iconPos);
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
