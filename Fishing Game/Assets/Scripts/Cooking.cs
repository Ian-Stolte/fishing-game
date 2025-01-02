using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Cooking : MonoBehaviour
{
    [SerializeField] private GameObject emptyBox;
    [SerializeField] private Transform fishParent;
    [SerializeField] private Transform veggieParent;

    private FishTracker fishTracker;


    private void Awake()
    {
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
    }


    private void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Raycast!");
            RaycastHit2D[] hits = Physics2D.RaycastAll(Input.mousePosition, Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject.name.Contains("Box(Clone)"))
                    AddToPot(hit.collider.gameObject);
            }
        }*/

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
                Debug.Log(result.gameObject.name);
                if (result.gameObject.name.Contains("Box(Clone)"))
                    AddToPot(result.gameObject);
            }
        }
    }

    private void AddToPot(GameObject box)
    {
        Debug.Log(box.name.Substring(0, box.name.Length-12));
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
        for (int i = 0; i < 12; i++) //TODO: change to vegetables/other ingredients
        {
            GameObject box = null;
            if (fishTracker.fish[i].boxSprite == null)
            {
                box = Instantiate(emptyBox, Vector2.zero, Quaternion.identity, veggieParent);
            }
            else
            {
                box = Instantiate(fishTracker.fish[i].boxSprite, Vector2.zero, Quaternion.identity, veggieParent);
                box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + fishTracker.fish[i].Quantity();
                if (fishTracker.fish[i].Quantity() == 0)
                    box.transform.GetChild(3).gameObject.SetActive(true);
            }
            box.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 385-70*i);
        }
    }
}
