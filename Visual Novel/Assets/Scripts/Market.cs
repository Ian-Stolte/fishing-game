using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour
{
    [SerializeField] private GameObject inventoryBox;
    [SerializeField] private Transform inventoryParent;
    public bool visitedToday;

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
            //choose daily deals
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
                if (sortedFish[i].Quantity() == 0)
                {
                    box.transform.GetChild(3).gameObject.SetActive(true); //show disable filter
                }
                //show border if daily deal
            }
            else
            {
                box = Instantiate(inventoryBox, Vector3.zero, Quaternion.identity, inventoryParent);
            }
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100 + 75*(i%4), 70 - 75*(i/4));
        }
    }
}
