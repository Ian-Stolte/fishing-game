using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public int time;
    public int totalTime;
    [SerializeField] private GameObject emptyPrefab;

    public void CollectPlant(string type)
    {
        Food f = GameObject.Find("Food Tracker").GetComponent<FoodTracker>().food.FirstOrDefault(f => f.name == type);
        f.totalFound++;
        f.quantity++;
        GameObject emptyBox = Instantiate(emptyPrefab, transform.position, Quaternion.identity, transform.parent);
        emptyBox.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        Destroy(gameObject);
    }
}