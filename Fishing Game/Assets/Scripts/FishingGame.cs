using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FishingGame : MonoBehaviour
{
    public int cheapBait;
    [SerializeField] private TextMeshProUGUI cheapBaitText;
    [SerializeField] private GameObject buyCheapBait;
    [SerializeField] private GameObject badBG;
    
    public int bait;
    [SerializeField] private TextMeshProUGUI baitText;
    [SerializeField] private GameObject buyBait;
    [SerializeField] private GameObject goodBG;
    
    private GameObject dragSprite;
    private GameObject chosenBait;
    private bool flyingBack;
    private enum BaitType
    {
        CHEAP,
        GOOD,
        NONE
    }
    private BaitType activeBait;
    [SerializeField] private RectTransform baitCircle;

    [SerializeField] private GameObject castButton;
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject fishPopup;
    [SerializeField] private GameObject failPopup;
    [SerializeField] private TextMeshProUGUI fishQuality;
    [SerializeField] private TextMeshProUGUI fishType;
    [SerializeField] private TextMeshProUGUI fishPrice;

    [SerializeField] private GameObject clickButton;

    [SerializeField] private TextMeshProUGUI statusTxt;
    [SerializeField] private Animator biteTxt;
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
    private PlayerManager player;


    void Awake()
    {
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
        player = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
    }

    void OnEnable()
    {
        fishPopup.SetActive(false);
        failPopup.SetActive(false);
        statusTxt.gameObject.SetActive(false);
        castButton.SetActive(true);
        baitText.text = "" + bait;
        cheapBaitText.text = "" + cheapBait;
        activeBait = BaitType.NONE;
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
                if (yVal < 15 && goodBG.activeSelf)
                {
                    fishQuality.text = "High-Quality catch!";
                    fishTracker.CatchFish(Random.Range(0f, 1f) < rarePct, 2);
                    fishQuality.color = qualityColors[0];
                }
                else if ((yVal < 40 && goodBG.activeSelf) || yVal < 15)
                {
                    fishQuality.text = "Medium-Quality catch";
                    fishTracker.CatchFish(false, 1);
                    fishQuality.color = qualityColors[1];
                }
                else if ((yVal < 70 && goodBG.activeSelf) || yVal < 50)
                {
                    fishQuality.text = "Low-Quality catch";
                    fishTracker.CatchFish(false, 0);
                    fishQuality.color = qualityColors[2];
                }
                else
                {
                    failPopup.SetActive(true);
                }
                goodBG.SetActive(false);
                badBG.SetActive(false);
                slider.SetActive(false);
                fishPopup.SetActive(true);
                Destroy(baitCircle.GetChild(baitCircle.childCount-1).gameObject);
            }
            else if (fishPopup.activeSelf || failPopup.activeSelf)
            {
                fishPopup.SetActive(false);
                failPopup.SetActive(false);
                castButton.SetActive(true);
                //GameObject.Find("Location BG").GetComponent<EventPlayer>().readyToReturn = true;
                clickButton.SetActive(true);
            }
        }
        else if (slider.GetComponent<RectTransform>().anchoredPosition.y < -100 && moving)
        {
            moving = false;
            failPopup.SetActive(true);
        }


        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current) {position = Input.mousePosition };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name.Contains("Bait Text") && activeBait == BaitType.NONE && !flyingBack && !goodBG.activeSelf && !badBG.activeSelf)
                {
                    chosenBait = result.gameObject;
                    DragOffShelf();
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && dragSprite != null)
        {
            Vector2 localPos = baitCircle.InverseTransformPoint(dragSprite.GetComponent<RectTransform>().position);
            if (baitCircle.rect.Contains(localPos))
            {
                dragSprite.transform.SetParent(baitCircle);
                dragSprite.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                dragSprite = null;
                if (chosenBait.name.Contains("Cheap"))
                {
                    activeBait = BaitType.CHEAP;
                    cheapBait--;
                }
                else
                {
                    activeBait = BaitType.GOOD;
                    bait--;
                }
                castButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                if (chosenBait.name.Contains("Cheap"))
                    chosenBait.GetComponent<TextMeshProUGUI>().text = "" + cheapBait;
                else
                    chosenBait.GetComponent<TextMeshProUGUI>().text = "" + bait;
                StartCoroutine(FlyBack());
            }
        }

        if (dragSprite != null && !flyingBack)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, null, out Vector2 localPoint);
            dragSprite.GetComponent<RectTransform>().localPosition = localPoint;
        }
    }

    private void DragOffShelf()
    {
        int quantity = int.Parse(chosenBait.GetComponent<TextMeshProUGUI>().text);
        if (quantity > 0)
        {
            chosenBait.GetComponent<TextMeshProUGUI>().text = "" + (quantity-1);
            dragSprite = Instantiate(chosenBait.transform.GetChild(0).gameObject, Vector2.zero, chosenBait.transform.GetChild(0).rotation, transform);
        }
    }

    private IEnumerator FlyBack()
    {
        flyingBack = true;
        Vector2 target = chosenBait.GetComponent<RectTransform>().position;
        Vector2 start = dragSprite.GetComponent<RectTransform>().position;
        for (float i = 0; i < 1; i += 0.08f)
        {
            dragSprite.GetComponent<RectTransform>().position = Vector2.Lerp(start, target, i);
            if (dragSprite.GetComponent<Image>() != null)
                dragSprite.GetComponent<Image>().color = new Color(1, 1, 1, 1-(i/2));
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(dragSprite);
        flyingBack = false;
    }


    public void CastLine()
    {
        if (activeBait != BaitType.NONE)
        {
            castButton.SetActive(false);
            goodBG.SetActive(activeBait == BaitType.GOOD);
            badBG.SetActive(activeBait == BaitType.CHEAP);
            StartCoroutine(DelayedCast(Random.Range(4f, 8f)));
            slider.SetActive(true);
        }
        activeBait = BaitType.NONE;
        castButton.GetComponent<Button>().interactable = false; 
    }


    private IEnumerator DelayedCast(float waitTime)
    {
        slider.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
        direction = 1;
        ellipses = "";
        statusTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitTime-1);
        statusTxt.gameObject.SetActive(false);
        biteTxt.Play("Bite!FadeOut");
        yield return new WaitForSeconds(1);
        clickButton.SetActive(false);
        moving = true;
    }


    public void BuyBait(bool cheap)
    {
        if (cheap)
        {
            if (player.money >= 1)
            {
                player.money--;
                cheapBait++;
                cheapBaitText.text = "" + cheapBait;
            }
        }
        else
        {
            if (player.money >= 3)
            {
                player.money -= 3;
                bait++;
                baitText.text = "" + bait;
            }
        }
        if (player.money < 1)
            buyCheapBait.GetComponent<Button>().interactable = false;
        if (player.money < 3)
            buyBait.GetComponent<Button>().interactable = false;
    }
}