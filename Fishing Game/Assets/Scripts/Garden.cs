using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Garden : MonoBehaviour
{
    public Seed[] seeds;
    public SeedData[,] seedData = new SeedData[4, 12];
    public Color[] comboColors;

    [SerializeField] private Transform plants;
    [SerializeField] private Transform seedBoxes;
    [SerializeField] private GameObject currentHover;

    private GameObject box;
    private GameObject dragSprite;
    private bool flyingBack;
    
    [SerializeField] private PlayerManager player;


    private void Awake()
    {
        foreach (Transform child in plants)
        {
            Plant p = child.GetComponent<Plant>();
            SeedData s = new SeedData(child.gameObject, "Empty");
            seedData[p.index/7, p.index%7] = s;
        }
    }

    private void OnEnable()
    {
        UpdateCounts();
    }

    public void UpdateCounts()
    {
        foreach (Transform child in seedBoxes)
        {
            Seed s = seeds.FirstOrDefault(s => s.name == child.name);
            child.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + s.quantity;
            child.GetChild(2).gameObject.SetActive(s.quantity == 0);
        }
        for (int i = 0; i < 28; i++)
        {
            seedData[i/7, i%7].old = true;
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && dragSprite != null)
        {
            bool boxFound = false;
            foreach (Transform child in plants)
            {
                Vector2 localPos = child.InverseTransformPoint(dragSprite.GetComponent<RectTransform>().position);
                if (child.GetComponent<RectTransform>().rect.Contains(localPos) && child.name.Contains("Empty Square") && child.GetComponent<CanvasGroup>().alpha == 1)
                {
                    boxFound = true;
                    PlantSeeds(box.name, child);
                    Destroy(dragSprite);
                }
            }
            if (!boxFound)
            {
                int quantity = int.Parse(box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
                box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + (quantity+1);
                box.transform.GetChild(2).gameObject.SetActive(false);
                StartCoroutine(FlyBack());
            }
        }

        if (dragSprite != null && !flyingBack)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, null, out Vector2 localPoint);
            dragSprite.GetComponent<RectTransform>().localPosition = localPoint;
        }
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        if (currentHover != null)
            currentHover.SetActive(false);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.transform.parent == plants && result.gameObject.name.Contains("Empty Square"))
            {
                if (result.gameObject.GetComponent<CanvasGroup>().alpha == 1 && dragSprite != null && !flyingBack) //filter
                {
                    currentHover = result.gameObject.transform.GetChild(1).gameObject;
                    currentHover.SetActive(true);
                }
                else if (result.gameObject.GetComponent<CanvasGroup>().alpha < 1 && (dragSprite == null || flyingBack)) //hover price
                {
                    currentHover = result.gameObject.transform.GetChild(2).gameObject;
                    currentHover.SetActive(true);
                    TextMeshProUGUI txt = currentHover.GetComponent<TextMeshProUGUI>(); 
                    int cost = int.Parse(txt.text.Substring(0, txt.text.Length-4));
                    if (player.money >= cost)
                    {
                        txt.color = new Color(1, 1, 1);
                        if (Input.GetMouseButtonDown(0))
                        {
                            player.money -= cost;
                            result.gameObject.GetComponent<CanvasGroup>().alpha = 1;
                            if (result.gameObject.transform.childCount > 3)
                            {
                                foreach (Transform child in result.gameObject.transform)
                                {
                                    if (child.GetSiblingIndex() > 2)
                                        child.gameObject.SetActive(false);
                                }
                            }
                            currentHover.SetActive(false);
                        }
                    }
                    else
                        txt.color = new Color32(238, 133, 133, 255);
                }
            }
            if (result.gameObject.transform.parent == seedBoxes && Input.GetMouseButtonDown(0)) //drag out seeds
            {
                box = result.gameObject;
                DragOffShelf();
            }
        }
    }


    private void DragOffShelf()
    {
        int quantity = int.Parse(box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        if (quantity > 0)
        {
            box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + (quantity-1);
            if (quantity-1 == 0)
                box.transform.GetChild(2).gameObject.SetActive(true);
            
            dragSprite = Instantiate(box.transform.GetChild(0).gameObject, box.transform.position, box.transform.GetChild(0).rotation, transform);
        }
    }


    private IEnumerator FlyBack()
    {
        if (currentHover != null)
            currentHover.SetActive(false);
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


    private void PlantSeeds(string type, Transform emptySquare)
    {
        Seed s = seeds.FirstOrDefault(s => s.name == type);
        s.quantity--;
        GameObject plant = Instantiate(s.plantSquare, emptySquare.position, Quaternion.identity, emptySquare.parent);
        plant.GetComponent<RectTransform>().anchoredPosition = emptySquare.GetComponent<RectTransform>().anchoredPosition;
        plant.transform.localScale = new Vector3(0.68f, 0.68f, 0.68f);

        int index = emptySquare.GetComponent<Plant>().index;
        int col = index/7;
        int adjCol = (col > 1) ? 5-col : 1-col; //0 <-> 1,  2 <-> 3
        plant.GetComponent<Plant>().index = index;
        seedData[col, index%7] = new SeedData(plant, type);

        bool adjCarrot = AdjCarrot(col, adjCol, index%7, plant);
        bool alreadyBad = false;
        if (index % 7 != 0)
        {
            if (type == "Wheat")
                WheatCheck(col, adjCol, index%7, (index-1)%7, plant);
            else if (type == "Carrot" && !adjCarrot)
                CarrotCheck(col, adjCol, index%7, (index-1)%7, plant);
            else if (type == "Potato")
                PotatoCheck(col, adjCol, index%7, (index-1)%7, plant);
            else if (type == "Onion")
                alreadyBad = OnionCheck(col, adjCol, index%7, (index-1)%7, plant, false);
        }
        if (index % 7 != 6)
        {
            if (type == "Wheat")
                WheatCheck(col, adjCol, index%7, (index+1)%7, plant);
            else if (type == "Carrot" && !adjCarrot)
                CarrotCheck(col, adjCol, index%7, (index+1)%7, plant);
            else if (type == "Potato")
                PotatoCheck(col, adjCol, index%7, (index+1)%7, plant);
            else if (type == "Onion")
                OnionCheck(col, adjCol, index%7, (index+1)%7, plant, alreadyBad);
        }
        Destroy(emptySquare.gameObject);
        StartCoroutine(StartTimer(plant));
    }

    private void WheatCheck(int col, int adjCol, int index, int adjIndex, GameObject plant)
    {
        if (seedData[col, adjIndex].kind == "Wheat" && !seedData[col, adjIndex].old && !(seedData[adjCol, index].kind == "Wheat"))
        {
            plant.GetComponent<Image>().color = comboColors[0];
            seedData[col, adjIndex].square.GetComponent<Image>().color = comboColors[0];
        }
    }

    private bool AdjCarrot(int col, int adjCol, int index, GameObject plant)
    {
        if (seedData[adjCol, index].kind == "Carrot")
            return true;
        
        if (index != 0)
        {
            if (seedData[col, index-1].kind == "Carrot")
                return true;
        }
        if (index != 6)
        {
            if (seedData[col, index+1].kind == "Carrot")
                return true;
        }
        return false;
    }

    private void CarrotCheck(int col, int adjCol, int index, int adjIndex, GameObject plant)
    {
        if (seedData[adjCol, adjIndex].kind == "Carrot" && !seedData[adjCol, adjIndex].old)
        {
            plant.GetComponent<Image>().color = comboColors[1];
            seedData[adjCol, adjIndex].square.GetComponent<Image>().color = comboColors[1];
        }
    }

    private void PotatoCheck(int col, int adjCol, int index, int adjIndex, GameObject plant)
    {
        bool sameCol = seedData[col, adjIndex].kind == "Potato" && !seedData[col, adjIndex].old;
        bool diffCol = seedData[adjCol, adjIndex].kind == "Potato" && !seedData[adjCol, adjIndex].old;

        if (seedData[adjCol, index].kind == "Potato" && !seedData[adjCol, index].old)
        {
            plant.GetComponent<Image>().color = comboColors[2];
            seedData[adjCol, index].square.GetComponent<Image>().color = comboColors[2];
            if (sameCol)
                seedData[col, adjIndex].square.GetComponent<Image>().color = comboColors[2];
            if (diffCol)
                seedData[adjCol, adjIndex].square.GetComponent<Image>().color = comboColors[2];
        }
        else if (sameCol && diffCol)
        {
            plant.GetComponent<Image>().color = comboColors[2];
        }
    }

    private bool OnionCheck(int col, int adjCol, int index, int adjIndex, GameObject plant, bool alreadyBad)
    {
        bool sameCol = seedData[col, adjIndex].kind == "Onion";
        bool diffCol = seedData[adjCol, adjIndex].kind == "Onion";
        bool sameRow = seedData[adjCol, index].kind == "Onion";
        
        if (!sameCol && !diffCol && !sameRow && !alreadyBad)
        {
            plant.GetComponent<Image>().color = comboColors[3];
            return false;
        }
        else
        {
            plant.GetComponent<Image>().color = comboColors[4];
            if (sameCol)
                seedData[col, adjIndex].square.GetComponent<Image>().color = comboColors[4];
            if (diffCol)
                seedData[adjCol, adjIndex].square.GetComponent<Image>().color = comboColors[4];
            if (sameRow)
                seedData[adjCol, index].square.GetComponent<Image>().color = comboColors[4];
            return true;
        }
    }


    private IEnumerator StartTimer(GameObject plant)
    {
        float goalFill = 1.0f/plant.GetComponent<Plant>().totalTime;
        for (float i = 0; i < 1; i += 0.01f)
        {
            plant.transform.GetChild(2).GetChild(1).GetComponent<Image>().fillAmount = i * goalFill;
            yield return new WaitForSeconds(0.01f);
        }
    }
}


[System.Serializable]
public class Seed
{
    public string name;
    public int quantity;
    public int price;
    public string description;

    public GameObject plantSquare;
    public GameObject marketBox;
}


[System.Serializable]
public class SeedData
{
    public SeedData(GameObject square_, string kind_)
    {
        square = square_;
        kind = kind_;
        old = false;
    }

    public string kind;
    public GameObject square;
    public bool old;
}