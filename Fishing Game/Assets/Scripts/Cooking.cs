using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cooking : MonoBehaviour
{
    [SerializeField] private GameObject emptyBox;
    [SerializeField] private Transform fishParent;
    [SerializeField] private Transform veggieParent;

    private FishTracker fishTracker;

    private void Awake()
    {
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
    }


    private void OnEnable()
    {
        for (int i = 0; i < 12; i++)
        {
            GameObject box = null;
            if (fishTracker.fish[i].boxSprite == null)
            {
                box = Instantiate(emptyBox, Vector2.zero, Quaternion.identity, fishParent);
            }
            else
            {
                box = Instantiate(fishTracker.fish[i].boxSprite, Vector2.zero, Quaternion.identity, fishParent);
                box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + fishTracker.fish[i].Quantity();
                if (fishTracker.fish[i].Quantity() == 0)
                    box.transform.GetChild(3).gameObject.SetActive(true);
            }
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 385-70*i);
        }
        for (int i = 0; i < 12; i++) //TODO: change to vegetables/other ingredients
        {
            GameObject box = null;
            if (fishTracker.fish[i].boxSprite == null)
            {
                box = Instantiate(emptyBox, Vector2.zero, Quaternion.identity, veggieParent);
            }
            else
            {
                box = Instantiate(fishTracker.fish[i].boxSprite, Vector2.zero, Quaternion.identity, veggieParent);
                box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + fishTracker.fish[i].Quantity();
                if (fishTracker.fish[i].Quantity() == 0)
                    box.transform.GetChild(3).gameObject.SetActive(true);
            }
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 385-70*i);
        }
    }
}
