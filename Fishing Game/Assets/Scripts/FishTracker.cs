using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FishTracker : MonoBehaviour
{
    public Fish[] fish;
    [HideInInspector] public List<Fish> rareFish;
    [HideInInspector] public List<Fish> commonFish;

    [SerializeField] TextMeshProUGUI fishNameTxt;
    [SerializeField] TextMeshProUGUI fishPriceTxt;
    [SerializeField] private Transform fishSprite;
    [SerializeField] private Color rareColor;

    [SerializeField] private bool showAllCaught;


    private void Start()
    {
        foreach (Fish f in fish)
        {
            if (f.rarity == Fish.Rarity.COMMON && f.sprite != null)
                commonFish.Add(f);
            else if (f.rarity == Fish.Rarity.RARE && f.sprite != null)
                rareFish.Add(f);
        }
    }

    public void CatchFish(bool rare, int quality)
    {
        Fish caughtFish = commonFish[0];
        if (rare)
        {
            caughtFish = rareFish[Random.Range(0, rareFish.Count)]; //change to calc by weather
            fishNameTxt.color = rareColor;
        }
        else
        {
            caughtFish = commonFish[Random.Range(0, commonFish.Count)]; //change to calc by weather
            fishNameTxt.color = new Color(255, 255, 255);
        }
        fishNameTxt.text = "" + caughtFish.name;
        foreach (Fish f in fish)
        {
            if (f.name == caughtFish.name)
            {
                if (quality == 0) //low
                {
                    f.currentTotal.x++;
                    fishPriceTxt.text = "<b>" + (int)Mathf.Round(f.price * 0.3f) + " sp.";
                }
                else if (quality == 1) //medium
                {
                    f.currentTotal.y++;
                    fishPriceTxt.text = "<b>" + (int)Mathf.Round(f.price * 0.6f) + " sp.";
                }
                else if (quality == 2) //high
                {
                    f.currentTotal.z++;
                    fishPriceTxt.text = "<b>" + f.price + " sp.";
                }
                f.totalCaught++;
                foreach (Transform child in fishSprite)
                {
                    Destroy(child.gameObject);
                }
                GameObject sprite = Instantiate(f.sprite, Vector3.zero, Quaternion.identity, fishSprite);
                sprite.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
        }
    }

    public Fish[] SortByQuantity()
    {
        
        List<Fish> discovered = new List<Fish>();
        foreach (Fish f in fish)
        {
            if ((showAllCaught && f.totalCaught > 0) || f.Quantity() > 0)
            {
                discovered.Add(f);
            }
        }
        Fish[] arr = discovered.ToArray();

        for (int i = 1; i < arr.Length; i++)
        {
            int curr = i;
            while (arr[curr-1].Quantity() < arr[curr].Quantity())
            {
                Fish temp = arr[curr-1];
                arr[curr-1] = arr[curr];
                arr[curr] = temp;
                if (curr > 1)
                    curr--;
                else
                    break;
            }
        }
        return arr;
    }

    public Fish RandomFish() //add bool to choose rarity (e.g so we don't have deals on 3 rare fish and 0 common)?
    {
        Fish f = fish[Random.Range(0, fish.Length)];
        while (f.sprite == null || f.dealPrice != 0)
        {
            f = fish[Random.Range(0, fish.Length)];
        }
        return f;
    }
}


[System.Serializable]
public class Fish
{
    public string name;
    public float[] spawnRate; //sunny, rainy, windy
    public enum Rarity{COMMON, RARE};
    public Rarity rarity;

    public int totalCaught;
    public Vector3 currentTotal;
    public int Quantity() { return (int)currentTotal.x + (int)currentTotal.y + (int)currentTotal.z; }

    [HideInInspector] public int dealPrice;
    public int price;

    public GameObject sprite;
    public GameObject boxSprite;
}