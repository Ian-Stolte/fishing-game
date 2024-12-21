using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTracker : MonoBehaviour
{
    public Fish[] fish;
    [HideInInspector] public List<Fish> rareFish;
    [HideInInspector] public List<Fish> commonFish;

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

    public void CatchFish(bool rare, int quality, TMPro.TextMeshProUGUI txtField)
    {
        Fish caughtFish = commonFish[0];
        if (rare)
        {
            caughtFish = rareFish[Random.Range(0, rareFish.Count)]; //change to calc by weather
            txtField.text = "You caught a rare fish: a <b>" + caughtFish.name + "</b>!";
        }
        else
        {
            caughtFish = commonFish[Random.Range(0, commonFish.Count)]; //change to calc by weather
            txtField.text = "You caught a <b>" + caughtFish.name + "</b>";
        }
        foreach (Fish f in fish)
        {
            if (f.name == caughtFish.name)
            {
                if (quality == 0) //low
                    f.currentTotal.x++;
                else if (quality == 1) //medium
                    f.currentTotal.y++;
                else if (quality == 2) //high
                    f.currentTotal.z++;
                f.totalCaught++;
            }
        }
    }

    public Fish[] SortByQuantity()
    {
        
        List<Fish> discovered = new List<Fish>();
        foreach (Fish f in fish)
        {
            if (f.totalCaught > 0)
            {
                discovered.Add(f);
            }
        }
        Fish[] arr = discovered.ToArray();

        for (int i = 1; i < arr.Length; i++)
        {
            int curr = i;
            Debug.Log(arr[curr].name + " : " + arr[curr].Quantity());
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

    //public int currentPrice;
    public GameObject sprite;
}