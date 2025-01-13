using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTracker : MonoBehaviour
{
    public Food[] food;
}


[System.Serializable]
public class Food
{
    public string name;

    public int totalFound;
    public int quantity;
    public int price;
    public string description;

    public GameObject boxSprite;
}