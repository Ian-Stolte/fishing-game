using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Market : MonoBehaviour
{
    [SerializeField] private GameObject inventoryBox;
    [SerializeField] private Transform inventoryParent;
    
    [SerializeField] private TextMeshProUGUI moneyTxt;
    [SerializeField] private GameObject moneyPopup;

    [SerializeField] private Transform deals;
    [SerializeField] private GameObject sellPopup;
    [SerializeField] private Color dealColor;
    [SerializeField] private Color priceColor;
    public bool visitedToday;

    private Fish[] sortedFish;
    private Fish hoveredFish;
    private Transform hoveredBox;

    [SerializeField] private int numDeals;
    
    [SerializeField] private PlayerManager player;
    [SerializeField] private FishTracker fishTracker;


    void OnEnable()
    {
        if (!visitedToday)
        {
            visitedToday = true;
            //set daily deals
            foreach (Fish f in fishTracker.fish)
                f.dealPrice = 0;
            foreach (Transform child in deals)
                child.gameObject.SetActive(false);

            for (int i = 0; i < numDeals; i++)
            {
                Fish dealFish = fishTracker.RandomFish();
                deals.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = dealFish.name;
                int dealPrice = (int)Mathf.Round(dealFish.price * Random.Range(1.5f, 2.0f));
                deals.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text = dealPrice + " sp.";
                dealFish.dealPrice = dealPrice;
                deals.GetChild(i).gameObject.SetActive(true);
            }
        }   

        foreach (Transform child in inventoryParent)
        {
            Destroy(child.gameObject);
        }
        
        sortedFish = fishTracker.SortByQuantity();
        for (int i = 0; i < 12; i++)
        {
            GameObject box = null;
            if (i < sortedFish.Length)
            {
                box = Instantiate(sortedFish[i].sprite, Vector3.zero, Quaternion.identity, inventoryParent);
                box.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "" + sortedFish[i].Quantity();
                if (sortedFish[i].dealPrice != 0)
                {
                    box.transform.GetChild(2).gameObject.SetActive(true); //show border if daily deal
                }
                if (sortedFish[i].Quantity() == 0)
                {
                    box.transform.GetChild(3).gameObject.SetActive(true); //show disable filter
                }
                int index = i;
                box.GetComponent<Button>().onClick.AddListener(() => ShowSellPopup(index));
            }
            else
            {
                box = Instantiate(inventoryBox, Vector3.zero, Quaternion.identity, inventoryParent);
            }
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100 + 75*(i%4), 70 - 75*(i/4));
        }
        sellPopup.SetActive(false);
        moneyTxt.text = "Money: <b>" + player.money;
    }

    public void ShowSellPopup(int n)
    {
        hoveredFish = sortedFish[n];
        hoveredBox = inventoryParent.GetChild(n);
        sellPopup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = hoveredFish.name;
        if (hoveredFish.dealPrice != 0)
        {
            sellPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = hoveredFish.dealPrice + " sp.";
            sellPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = dealColor;
        }
        else
        {
            sellPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + hoveredFish.price + " sp.";
            sellPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = priceColor;
        }
        SetSellQuantities();
        sellPopup.SetActive(true);
    }

    public void SellFish(int n)
    {
        foreach (Fish f in fishTracker.fish)
        {
            if (f.name == hoveredFish.name)
            {
                int price = 0;
                if (n == 0)
                {
                    f.currentTotal.z--;
                    price = Mathf.Max(f.dealPrice, f.price);
                }
                else if (n == 1)
                {
                    f.currentTotal.y--;
                    price = (int)Mathf.Round(Mathf.Max(f.dealPrice, f.price) * 0.6f);
                }
                else if (n == 2)
                {
                    f.currentTotal.x--;
                    price = (int)Mathf.Round(Mathf.Max(f.dealPrice, f.price) * 0.3f);
                }
                hoveredBox.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + hoveredFish.Quantity();
                if (hoveredFish.Quantity() == 0)
                    hoveredBox.GetChild(3).gameObject.SetActive(true);
                SetSellQuantities();
                player.money += price;
                moneyTxt.text = "Money: <b>" + player.money;
                GameObject popup = Instantiate(moneyPopup, Vector3.zero, Quaternion.identity, transform);
                popup.GetComponent<TextMeshProUGUI>().text = "+" + price;
                break;
            }
        }
    }

    private void SetSellQuantities()
    {
        sellPopup.transform.GetChild(3).GetChild(2).GetComponent<Button>().interactable = (hoveredFish.currentTotal.z > 0);
        sellPopup.transform.GetChild(4).GetChild(2).GetComponent<Button>().interactable = (hoveredFish.currentTotal.y > 0);
        sellPopup.transform.GetChild(5).GetChild(2).GetComponent<Button>().interactable = (hoveredFish.currentTotal.x > 0);
        
        sellPopup.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + hoveredFish.currentTotal.z; //high
        sellPopup.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + hoveredFish.currentTotal.y; //medium
        sellPopup.transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + hoveredFish.currentTotal.x; //low
    }
}
