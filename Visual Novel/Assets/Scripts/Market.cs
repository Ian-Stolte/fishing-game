using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Market : MonoBehaviour
{
    [SerializeField] private GameObject inventoryBox;
    [SerializeField] private Transform inventoryParent;
    [SerializeField] private Transform deals;
    public bool visitedToday;

    [SerializeField] private int numDeals;
    [SerializeField] private FishTracker fishTracker;


    void Start()
    {
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
    }

    void OnEnable()
    {
        if (!visitedToday)
        {
            visitedToday = true;
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
        
        Fish[] sortedFish = fishTracker.SortByQuantity();
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
            }
            else
            {
                box = Instantiate(inventoryBox, Vector3.zero, Quaternion.identity, inventoryParent);
            }
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100 + 75*(i%4), 70 - 75*(i/4));
        }
    }
}
