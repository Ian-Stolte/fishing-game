using System.Collections;
using System.Linq;
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
    [SerializeField] private GameObject recipePopup;

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
            if (recipePopup.activeSelf)
                recipePopup.SetActive(false);

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
            //remove 1 quantity from fishTracker / foodTracker --- take highest-quality fish
            if (quantity-1 == 0)
                box.transform.GetChild(3).gameObject.SetActive(true);
            string ing = box.name.Substring(0, box.name.Length-11);
            activeIngredients.Add(ing);
            if (fishTracker.fish.Any(f => f.name == ing))
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
        List<Recipe> possibleRecipes = recipes.Select(r => r.DeepCopy()).ToList();
        List<string> checkedIngs = new List<string>();

        foreach (string str in activeIngredients)
        {
            string ingName = str;
            if (fishTracker.fish.Any(f => f.name == str))
            {
                ingName = "Fish";
                //replace with logic for different types of fish, different quality levels...
            }
            if (!checkedIngs.Contains(ingName))
            {
                checkedIngs.Add(ingName);
                for (int i = possibleRecipes.Count-1; i >= 0; i--)
                {
                    if (possibleRecipes[i].necessaryIngs.Contains(ingName))
                    {
                        possibleRecipes[i].necessaryIngs.Remove(ingName);
                    }
                    else if (possibleRecipes[i].disallowedIngs.Contains(ingName))
                    {
                        //Debug.Log("REMOVING " + possibleRecipes[i].name + " because of " + ingName.ToUpper());
                        possibleRecipes.RemoveAt(i);
                    }
                }
            }
        }
        activeIngredients.Clear();
        cookButton.SetActive(false);

        Recipe chosenRecipe;
        possibleRecipes.RemoveAll(r => r.necessaryIngs.Count > 1);
        if (possibleRecipes.Count == 0)
        {
            chosenRecipe = recipes[1];
            Debug.Log("Dubious Fish Slurry");
        }
        else
        {
            //foreach (Recipe r in possibleRecipes)
            //    Debug.Log("One away: " + r.name);
            possibleRecipes.RemoveAll(r => r.necessaryIngs.Count > 0);
            if (possibleRecipes.Count == 0)
            {
                chosenRecipe = recipes[0];
                Debug.Log("Simple Boiled Fish");
            }
            else
            {
                chosenRecipe = possibleRecipes[0];
                Debug.Log(possibleRecipes[0].name);
            }
        }
        recipePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = chosenRecipe.name;
        recipePopup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = chosenRecipe.description;
        //instantiate recipe sprite
        recipePopup.SetActive(true);
    }
}



[System.Serializable]
public class Recipe
{
    public string name;
    public List<string> necessaryIngs;
    public List<string> disallowedIngs;
    public string description;

    //public GameObject sprite
    //add additional effects

    public Recipe DeepCopy()
    {
        return new Recipe 
        {
            name = this.name,
            necessaryIngs = new List<string>(this.necessaryIngs),
            disallowedIngs = new List<string>(this.disallowedIngs),
            description = this.description
        };
    }
}