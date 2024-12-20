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
            if (f.rarity == Fish.Rarity.COMMON)
                commonFish.Add(f);
            else if (f.rarity == Fish.Rarity.RARE)
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

    public int currentPrice;
}