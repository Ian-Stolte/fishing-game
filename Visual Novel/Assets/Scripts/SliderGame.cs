using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderGame : MonoBehaviour
{
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject fishPopup;
    [SerializeField] private TextMeshProUGUI fishQuality;
    [SerializeField] private TextMeshProUGUI fishType;

    [SerializeField] private float rarePct;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            slider.GetComponent<Animator>().speed = 0;
            GameObject.Find("Location BG").GetComponent<EventPlayer>().readyToReturn = true;

            float yVal = Mathf.Abs(slider.GetComponent<RectTransform>().anchoredPosition.y);
            if (yVal < 15)
            {
                fishQuality.text = "High-Quality catch!";
                fishTracker.CatchFish(Random.Range(0f, 1f) < rarePct, 2, fishType);
            }
            else if (yVal < 40)
            {
                fishQuality.text = "Medium-Quality catch";
                fishTracker.CatchFish(false, 1, fishType);
            }
            else if (yVal < 70)
            {
                fishQuality.text = "Low-Quality catch";
                fishTracker.CatchFish(false, 0, fishType);
            }
            else
            {
                fishQuality.text = "Fail!";
                fishType.text = "You didn't catch anything...";
            }
            fishPopup.SetActive(true);
        }
    }
}