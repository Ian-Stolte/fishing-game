using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Cooking : MonoBehaviour
{
    [SerializeField] private GameObject emptyBox;
    [SerializeField] private Transform fishParent;
    [SerializeField] private Transform foodParent;
    [SerializeField] private GameObject bubbles;

    [SerializeField] private GameObject cookButton;
    [SerializeField]private bool canCook;
    [SerializeField] private RectTransform potBounds;
    [SerializeField] private GameObject recipePopup;
    private GameObject dragSprite;
    bool flyingBack;

    [SerializeField] private GameObject clickButton;

    [SerializeField] private GameObject openButton;
    [SerializeField] private GameObject closeButton;

    [SerializeField] private List<string> activeIngredients;
    private GameObject box;
    private int quality;
    private int numFish;
    [SerializeField] private List<Recipe> recipes;

    private FishTracker fishTracker;
    private FoodTracker foodTracker;


    private void Awake()
    {
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
        foodTracker = GameObject.Find("Food Tracker").GetComponent<FoodTracker>();
    }

    private void OnEnable()
    {
        canCook = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (recipePopup.activeSelf)
            {
                recipePopup.SetActive(false);
                clickButton.SetActive(true);
            }

            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name.Contains("Box(Clone)"))
                {
                    box = result.gameObject;
                    DragOffShelf();
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && dragSprite != null)
        {
            Vector2 localPos = potBounds.InverseTransformPoint(dragSprite.GetComponent<RectTransform>().position);
            if (potBounds.rect.Contains(localPos))
            {
                AddToPot(dragSprite.name.Substring(0, dragSprite.name.Length-7));
                Destroy(dragSprite);
            }
            else
            {
                int quantity = int.Parse(box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
                box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + (quantity+1);
                if (box.transform.parent == fishParent)
                    box.transform.GetChild(3).gameObject.SetActive(false);
                else
                    box.transform.GetChild(2).gameObject.SetActive(false);
                StartCoroutine(FlyBack());
            }
            foodParent.parent.parent.GetComponent<ScrollRect>().enabled = true;
            fishParent.parent.parent.GetComponent<ScrollRect>().enabled = true;
        }

        if (dragSprite != null && !flyingBack)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, null, out Vector2 localPoint);
            dragSprite.GetComponent<RectTransform>().localPosition = localPoint;
        }
    }


    public void Setup()
    {
        foreach (Transform child in fishParent)
            Destroy(child.gameObject);
        foreach (Transform child in foodParent)
            Destroy(child.gameObject);
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
                    box.transform.GetChild(2).gameObject.SetActive(true);
            }
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 175-70*i);
        }
        StartCoroutine(OpenPanels());
    }


    private IEnumerator OpenPanels()
    {
        openButton.SetActive(false);
        for (float i = 0; i < 0.5f; i += 0.01f)
        {
            fishParent.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(-459, -359, i/0.5f), -77);
            foodParent.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(498, 398, i/0.5f), -77);
            potBounds.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Mathf.Lerp(-353, -223, i/0.5f));
            yield return new WaitForSeconds(0.01f);
        }
        closeButton.SetActive(true);
        cookButton.SetActive(canCook);
    }


    private IEnumerator FlyBack()
    {
        flyingBack = true;
        Vector2 target = box.GetComponent<RectTransform>().position;
        Vector2 start = dragSprite.GetComponent<RectTransform>().position;
        for (float i = 0; i < 1; i += 0.08f)
        {
            dragSprite.GetComponent<RectTransform>().position = Vector2.Lerp(start, target, i);
            if (dragSprite.GetComponent<Image>() != null)
                dragSprite.GetComponent<Image>().color = new Color(1, 1, 1, 1-(i/2));
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(dragSprite);
        flyingBack = false;
    }


    private void AddToPot(string ing)
    {
        activeIngredients.Add(ing);
        if (fishTracker.fish.Any(f => f.name == ing))
            cookButton.SetActive(true);
        
        StartCoroutine(Bubbles());
    }

    private IEnumerator Bubbles()
    {
        bubbles.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        bubbles.SetActive(false);
    }


    private void DragOffShelf()
    {
        int quantity = int.Parse(box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        if (quantity > 0)
        {
            string ing = box.name.Substring(0, box.name.Length-11);
            box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + (quantity-1);
            if (quantity-1 == 0)
            {
                if (box.transform.parent == fishParent)
                    box.transform.GetChild(3).gameObject.SetActive(true);
                else
                    box.transform.GetChild(2).gameObject.SetActive(true);
            }
            dragSprite = Instantiate(box.transform.GetChild(0).gameObject, Vector2.zero, box.transform.GetChild(0).rotation, transform);
        }
        fishParent.parent.parent.GetComponent<ScrollRect>().enabled = false;
        foodParent.parent.parent.GetComponent<ScrollRect>().enabled = false;
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
                numFish++;
                //replace with logic for different types of fish, different quality levels...
                Fish fish = fishTracker.fish.FirstOrDefault(f => f.name == str);
                if (fish.currentTotal.z > 0)
                {
                    quality = 2;
                    fish.currentTotal.z--;
                }
                else if (fish.currentTotal.y > 0)
                {
                    quality = Mathf.Max(quality, 1);
                    fish.currentTotal.y--;
                }
                else
                {
                    fish.currentTotal.x--;
                }
            }
            else
            {
                Food food = foodTracker.food.FirstOrDefault(f => f.name == str);
                food.quantity--;
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
        }
        else
        {
            //foreach (Recipe r in possibleRecipes)
            //    Debug.Log("One away: " + r.name);
            possibleRecipes.RemoveAll(r => r.necessaryIngs.Count > 0);
            if (possibleRecipes.Count == 0)
            {
                chosenRecipe = recipes[0];
            }
            else
            {
                chosenRecipe = possibleRecipes[0];
                foreach (Recipe rec in possibleRecipes)
                {
                    Recipe recipe = recipes.FirstOrDefault(r => r.name == rec.name);
                    if (recipe.necessaryIngs.Count > chosenRecipe.necessaryIngs.Count)
                        chosenRecipe = recipe;
                }
            }
        }
        if (numFish > 2)
        {
            recipePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Extra Hearty " + chosenRecipe.name;
            chosenRecipe.quantity.z++;
        }
        else if (numFish > 1)
        {
            recipePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Hearty " + chosenRecipe.name;
            chosenRecipe.quantity.y++;
        }
        else
        {
            recipePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = chosenRecipe.name;
            chosenRecipe.quantity.x++;
        }
        recipePopup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = chosenRecipe.description;
        
        //instantiate recipe sprite
        //show quality of recipe
        recipePopup.SetActive(true);
        clickButton.SetActive(false);
        quality = 0;
        numFish = 0;
    }


    public void Close()
    {
        StartCoroutine(ClosePanels());
    }


    private IEnumerator ClosePanels()
    {
        closeButton.SetActive(false);
        canCook = cookButton.activeSelf;
        cookButton.SetActive(false);
        for (float i = 0; i < 0.5f; i += 0.01f)
        {
            fishParent.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(-359, -459, i/0.5f), -77);
            foodParent.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(398, 498, i/0.5f), -77);
            potBounds.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Mathf.Lerp(-223, -353, i/0.5f));
            yield return new WaitForSeconds(0.01f);
        }
        openButton.SetActive(true);
    }
}



[System.Serializable]
public class Recipe
{
    public string name;
    public List<string> necessaryIngs;
    public List<string> disallowedIngs;
    public string description;

    public Vector3 quantity; //change to separate object w/ quality and heartiness included, ingredients omitted??

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