using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public int time;
    public int totalTime;
    public float seedDropPct;
    [SerializeField] private GameObject emptyPrefab;


    public void CollectPlant(string type)
    {
        Food f = GameObject.Find("Food Tracker").GetComponent<FoodTracker>().food.FirstOrDefault(f => f.name == type);
        f.totalFound++;
        f.quantity++;
        if (Random.Range(0f, 1f) < seedDropPct)
        {
            Seed s = GameObject.Find("Garden").GetComponent<Garden>().seeds.FirstOrDefault(s => s.name == type);
            s.quantity++;
            GameObject.Find("Garden").GetComponent<Garden>().UpdateCounts();
        }
        GameObject emptyBox = Instantiate(emptyPrefab, transform.position, Quaternion.identity, transform.parent);
        emptyBox.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        Destroy(gameObject);
    }
}