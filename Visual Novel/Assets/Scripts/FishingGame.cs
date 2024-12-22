using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FishingGame : MonoBehaviour
{
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject fishPopup;
    [SerializeField] private TextMeshProUGUI fishQuality;
    [SerializeField] private TextMeshProUGUI fishType;
    [SerializeField] private TextMeshProUGUI fishPrice;

    [SerializeField] private float rarePct;
    [SerializeField] private Color[] qualityColors;

    private FishTracker fishTracker;


    void Start()
    {
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
    }

    void OnEnable()
    {
        fishPopup.SetActive(false);
        slider.GetComponent<Animator>().Play("SlideUpDown");
        slider.GetComponent<Animator>().speed = 1;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            slider.GetComponent<Animator>().speed = 0;
            GameObject.Find("Location BG").GetComponent<EventPlayer>().readyToReturn = true;

            float yVal = Mathf.Abs(slider.GetComponent<RectTransform>().anchoredPosition.y);
            if (yVal < 15)
            {
                fishQuality.text = "High-Quality catch!";
                fishTracker.CatchFish(Random.Range(0f, 1f) < rarePct, 2);
                fishQuality.color = qualityColors[0];
            }
            else if (yVal < 40)
            {
                fishQuality.text = "Medium-Quality catch";
                fishTracker.CatchFish(false, 1);
                fishQuality.color = qualityColors[1];
            }
            else if (yVal < 70)
            {
                fishQuality.text = "Low-Quality catch";
                fishTracker.CatchFish(false, 0);
                fishQuality.color = qualityColors[2];
            }
            else
            {
                fishQuality.text = "Fail!";
                fishType.text = "You didn't catch anything...";
                //TODO: display separate layout for fail (no sprite, centered text)
                fishQuality.color = qualityColors[3];
            }
            fishPopup.SetActive(true);
        }
    }
}