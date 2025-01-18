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
    [SerializeField] private GameObject foodPopupPrefab;
    [SerializeField] private GameObject seedPopupPrefab;


    public void CollectPlant(string type)
    {
        Vector2 myPos = GetComponent<RectTransform>().anchoredPosition;
        Food f = GameObject.Find("Food Tracker").GetComponent<FoodTracker>().food.FirstOrDefault(f => f.name == type);
        f.totalFound++;
        f.quantity++;
        if (Random.Range(0f, 1f) < seedDropPct)
        {
            Seed s = GameObject.Find("Garden").GetComponent<Garden>().seeds.FirstOrDefault(s => s.name == type);
            s.quantity++;
            GameObject.Find("Garden").GetComponent<Garden>().UpdateCounts();
            GameObject seedPopup = Instantiate(seedPopupPrefab, transform.position, Quaternion.identity, transform.parent.parent);
            //seedPopup.GetComponent<RectTransform>().anchoredPosition = (myPos.x > 0) ? myPos + new Vector2(-57, -15) : myPos + new Vector2(80, -15);
            seedPopup.GetComponent<RectTransform>().anchoredPosition = myPos + new Vector2(10, -15);
            seedPopup.SetActive(true);
        }
        GameObject foodPopup = Instantiate(foodPopupPrefab, transform.position, Quaternion.identity, transform.parent.parent);
        //foodPopup.GetComponent<RectTransform>().anchoredPosition = (myPos.x > 0) ? myPos + new Vector2(-49, 20) : myPos + new Vector2(86, 20);
        foodPopup.GetComponent<RectTransform>().anchoredPosition = myPos + new Vector2(10, 20);
        foodPopup.SetActive(true);
        GameObject emptyBox = Instantiate(emptyPrefab, transform.position, Quaternion.identity, transform.parent);
        emptyBox.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        Destroy(gameObject);
    }
}