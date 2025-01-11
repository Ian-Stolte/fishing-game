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
    [SerializeField] private Transform plants;
    [SerializeField] private Transform seedBoxes;

    private GameObject box;
    private GameObject dragSprite;
    private bool flyingBack;


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
                if (result.gameObject.transform.parent == seedBoxes)
                {
                    box = result.gameObject;
                    DragOffShelf();
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && dragSprite != null)
        {
            bool boxFound = false;
            foreach (Transform child in plants)
            {
                Vector2 localPos = child.InverseTransformPoint(dragSprite.GetComponent<RectTransform>().position);
                if (child.GetComponent<RectTransform>().rect.Contains(localPos) && child.name.Contains("Empty Square"))
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
    }


    private void DragOffShelf()
    {
        int quantity = int.Parse(box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        if (quantity > 0)
        {
            box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + (quantity-1);
            if (quantity-1 == 0)
                box.transform.GetChild(2).gameObject.SetActive(true);
            
            dragSprite = Instantiate(box.transform.GetChild(0).gameObject, Vector2.zero, box.transform.GetChild(0).rotation, transform);
        }
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


    private void PlantSeeds(string type, Transform emptySquare)
    {
        Seed s = seeds.FirstOrDefault(s => s.name == type);
        s.quantity--;
        GameObject plant = Instantiate(s.plantSquare, emptySquare.position, Quaternion.identity, emptySquare.parent);
        plant.GetComponent<RectTransform>().anchoredPosition = emptySquare.GetComponent<RectTransform>().anchoredPosition;
        Destroy(emptySquare.gameObject);
        StartCoroutine(StartTimer(plant));
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
    public GameObject plantSquare;
    public GameObject marketBox;
}