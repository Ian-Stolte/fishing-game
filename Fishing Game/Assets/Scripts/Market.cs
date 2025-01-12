using System.Linq;
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

    [SerializeField] private GameObject[] stalls;
    private int index;
    private IEnumerator rotateCor;
    [SerializeField] private Transform sellBoxes;
    [SerializeField] private Transform sellPrices;

    [SerializeField] private int numDeals;
    
    [SerializeField] private PlayerManager player;
    [SerializeField] private Garden garden;
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

            //set seeds for sale
            foreach (Transform child in sellBoxes)
                Destroy(child.gameObject);

            int firstSeed = Random.Range(0, garden.seeds.Length);
            SpawnSeed(firstSeed, -250, 0);

            int secondSeed = Random.Range(0, garden.seeds.Length-1);
            if (secondSeed >= firstSeed)
                secondSeed++;
            SpawnSeed(secondSeed, -150, 1);
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
                box = Instantiate(sortedFish[i].boxSprite, Vector3.zero, Quaternion.identity, inventoryParent);
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

    private void SpawnSeed(int n, int xPos, int priceNum)
    {
        GameObject seedBox = Instantiate(garden.seeds[n].marketBox, Vector2.zero, Quaternion.identity, sellBoxes);
        seedBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 115);
        sellPrices.GetChild(priceNum).GetComponent<TextMeshProUGUI>().text = garden.seeds[n].price + " sp.";
        seedBox.transform.GetChild(1).gameObject.SetActive(false);
        seedBox.GetComponent<Button>().onClick.AddListener(() => BuyItem(garden.seeds[n].name + " Seeds"));
        //randomly set quantity?
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
                moneyTxt.text = "Money:  <b>" + player.money;
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


    public void SwapStall(int direction)
    {
        if (rotateCor != null)
            StopCoroutine(rotateCor);
        rotateCor = RotateMarket(direction);
        StartCoroutine(rotateCor);
    }

    private IEnumerator RotateMarket(int direction) // -1: left,  1: right
    {
        index = (index + direction + stalls.Length) % stalls.Length;
        stalls[(index - direction + stalls.Length) % stalls.Length].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        stalls[index].GetComponent<RectTransform>().anchoredPosition = new Vector2(850*direction, 0);
        for (float i = 0; i < 1; i += 0.02f)
        {
            stalls[(index - direction + stalls.Length) % stalls.Length].GetComponent<RectTransform>().anchoredPosition += new Vector2(30*direction*-1, 0);
            stalls[index].GetComponent<RectTransform>().anchoredPosition += new Vector2(17f*direction*-1, 0);
            yield return new WaitForSeconds(0.005f);
        }
        
    }


    private void BuyItem(string itemName)
    {
        if (itemName.Contains("Seeds"))
        {
            string seedType = itemName.Substring(0, itemName.Length-6);
            Seed s = garden.seeds.FirstOrDefault(s => s.name == seedType);
            if (player.money >= s.price)
            {
                s.quantity++;
                player.money -= s.price;
            }
        }
        UpdateSell();
    }

    private void UpdateSell()
    {
        moneyTxt.text = "Money: <b>" + player.money;
        for (int i = 0; i < sellBoxes.childCount; i++)
        {
            string price = sellPrices.GetChild(i).GetComponent<TextMeshProUGUI>().text;
            if (int.Parse(price.Substring(0, price.Length-3)) > player.money)
            {
                sellBoxes.GetChild(i).GetChild(2).gameObject.SetActive(true);
            }
        }
    }
}
