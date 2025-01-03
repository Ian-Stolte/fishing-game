using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Cooking : MonoBehaviour
{
    [SerializeField] private GameObject emptyBox;
    [SerializeField] private Transform fishParent;
    [SerializeField] private Transform foodParent;

    [SerializeField] private GameObject cookButton;

    [SerializeField] private List<string> activeIngredients;
    [SerializeField] private List<Recipe> recipes;

    private FishTracker fishTracker;
    private FoodTracker foodTracker;


    private void Awake()
    {
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
        foodTracker = GameObject.Find("Food Tracker").GetComponent<FoodTracker>();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name.Contains("Box(Clone)"))
                    AddToPot(result.gameObject);
            }
        }
    }

    private void AddToPot(GameObject box)
    {
        int quantity = int.Parse(box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        if (quantity > 0)
        {
            box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + (quantity-1);
            if (quantity-1 == 0)
                box.transform.GetChild(3).gameObject.SetActive(true);
            string ing = box.name.Substring(0, box.name.Length-11);
            //Debug.Log("ADDING TO POT: " + ing);
            activeIngredients.Add(ing);
            cookButton.SetActive(true);
        }
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
        for (int i = 0; i < 6; i++)
        {
            GameObject box = null;
            if (foodTracker.food[i].boxSprite == null)
            {
                box = Instantiate(emptyBox, Vector2.zero, Quaternion.identity, foodParent);
            }
            else
            {
                box = Instantiate(foodTracker.food[i].boxSprite, Vector2.zero, Quaternion.identity, foodParent);
                box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + foodTracker.food[i].quantity;
                if (foodTracker.food[i].quantity == 0)
                    box.transform.GetChild(3).gameObject.SetActive(true);
            }
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 175-70*i);
        }
    }

    
    public void Cook()
    {
        Debug.Log("Cooking a meal with:");
        foreach (string str in activeIngredients)
        {
            Debug.Log(str);
        }
        activeIngredients.Clear();
        cookButton.SetActive(false);
    }
}



[System.Serializable]
public class Recipe
{
    public string name;
    public List<string> ingredients;
    public string description;

    //public GameObject sprite
    //add additional effects
}