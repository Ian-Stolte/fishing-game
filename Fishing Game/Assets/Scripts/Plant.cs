using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plant : MonoBehaviour
{
    public bool empty;
    public int index;

    public int time;
    public int totalTime;
    public float seedDropPct;

    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private GameObject foodPopupPrefab;
    [SerializeField] private GameObject seedPopupPrefab;

    private Garden garden;


    void Awake()
    {
        garden = transform.parent.parent.GetComponent<Garden>();   
    }

    public void CollectSet()
    {
        string type = name.Substring(0, name.Length-7);
        CollectPlant(type);
        if (GetComponent<Image>().color != garden.comboColors[4] && type != "Onion")
        {
            Debug.Log("Part of a " + type + " set!");
            //find all other plants in the set and collect them
        }
    }

    public void CollectPlant(string type)
    {
        Vector2 myPos = GetComponent<RectTransform>().anchoredPosition;
        Food f = GameObject.Find("Food Tracker").GetComponent<FoodTracker>().food.FirstOrDefault(f => f.name == type);
        if (type == "Onion" && GetComponent<Image>().color != garden.comboColors[4])
        {
            f.totalFound += 2;
            f.quantity += 2;
        }
        else
        {
            f.totalFound++;
            f.quantity++;
        }
        if (Random.Range(0f, 1f) < seedDropPct)
        {
            Seed s = garden.seeds.FirstOrDefault(s => s.name == type);
            s.quantity++;
            garden.UpdateCounts();
            GameObject seedPopup = Instantiate(seedPopupPrefab, transform.position, Quaternion.identity, transform.parent.parent);
            seedPopup.GetComponent<RectTransform>().anchoredPosition = myPos + new Vector2(10, -15);
            seedPopup.SetActive(true);
        }
        GameObject foodPopup = Instantiate(foodPopupPrefab, transform.position, Quaternion.identity, transform.parent.parent);
        foodPopup.GetComponent<RectTransform>().anchoredPosition = myPos + new Vector2(10, 20);
        foodPopup.SetActive(true);
        if (type == "Onion" && GetComponent<Image>().color != garden.comboColors[4])
            foodPopup.transform.GetChild(1).gameObject.SetActive(true);
        GameObject emptyBox = Instantiate(emptyPrefab, transform.position, Quaternion.identity, transform.parent);
        emptyBox.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        emptyBox.GetComponent<Plant>().index = index;
        garden.seedData[index/7, index%7] = new SeedData(emptyBox, "Empty");
        Destroy(gameObject);
    }
}