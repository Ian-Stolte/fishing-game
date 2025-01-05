using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FishingGame : MonoBehaviour
{
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject fishPopup;
    [SerializeField] private GameObject failPopup;
    [SerializeField] private TextMeshProUGUI fishQuality;
    [SerializeField] private TextMeshProUGUI fishType;
    [SerializeField] private TextMeshProUGUI fishPrice;

    [SerializeField] private GameObject clickButton;

    private int timesCast;
    [SerializeField] private TextMeshProUGUI castsLeft;
    [SerializeField] private TextMeshProUGUI statusTxt;
    private string ellipses;
    private float ellpisesTimer;

    private bool moving;
    private int direction;
    [SerializeField] private float sliderSpeed;
    private float currentSpeed;
    private float speedModTimer;

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
        failPopup.SetActive(false);
        StartCoroutine(DelayedCast(Random.Range(3f, 10f)));
        timesCast = 1;
        castsLeft.text = "Casts Left:  <b>2";
    }

    void Update()
    {
        if (moving)
        {
            slider.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, direction*Time.deltaTime*currentSpeed);
            if (slider.GetComponent<RectTransform>().anchoredPosition.y > 100)
                direction = -1;

            speedModTimer -= Time.deltaTime;
            if (speedModTimer <= 0)
            {
                currentSpeed = Random.Range(sliderSpeed * 0.7f, sliderSpeed * 1.5f);
                speedModTimer = Random.Range(0.2f, 0.5f);
            }
        }
        else if (statusTxt.gameObject.activeSelf)
        {
            ellpisesTimer -= Time.deltaTime;
            if (ellpisesTimer <= 0)
            {
                ellipses += '.';
                if (ellipses.Length > 3)
                    ellipses = ".";
                ellpisesTimer = 1;
            }
            statusTxt.text = "Waiting for a bite" + ellipses;

        }

        if (Input.GetMouseButtonDown(0))
        {
            if (moving)
            {
                moving = false;

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
                    failPopup.SetActive(true);
                }
                fishPopup.SetActive(true);
            }
            else if (fishPopup.activeSelf || failPopup.activeSelf)
            {
                fishPopup.SetActive(false);
                failPopup.SetActive(false);
                timesCast++;
                if (timesCast > 3)
                {
                    GameObject.Find("Location BG").GetComponent<EventPlayer>().readyToReturn = true;
                    clickButton.SetActive(true);
                }
                else
                {
                    castsLeft.text = "Casts Left:  <b>" + (3-timesCast);
                    StartCoroutine(DelayedCast(Random.Range(3f, 10f)));
                }
            }
        }
        else if (slider.GetComponent<RectTransform>().anchoredPosition.y < -100 && moving)
        {
            moving = false;
            failPopup.SetActive(true);
        }
    }

    private IEnumerator DelayedCast(float waitTime)
    {
        clickButton.SetActive(true);
        slider.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
        direction = 1;
        ellipses = "";
        statusTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        statusTxt.gameObject.SetActive(false);
        clickButton.SetActive(false);
        moving = true;
    }
}