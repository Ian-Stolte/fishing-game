using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnClick : MonoBehaviour
{
    [SerializeField] private EventManager eventManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
            eventManager.UpdateInfo();
        }
    }
}
