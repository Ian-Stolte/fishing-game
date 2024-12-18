using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GameObject eventSummary;
    private int currentDay;
    private int currentLoop;

    private string[] locations = new string[]{"square", "rhombus", "hexagon", "circle"};

    public void SelectLocation(int n)
    {
        eventSummary.SetActive(true);
        eventSummary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You head to the " + locations[n] + "...";
        currentDay++;
        if (currentDay > 3)
        {
            currentDay = 1;
            currentLoop++;
        }
        timeText.GetComponent<TextMeshProUGUI>().text = "Day " + currentDay + ", Loop " + currentLoop;
    }
}
