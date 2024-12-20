using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventPlayer : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;

    private bool canClick = true;
    public bool readyToReturn;

    [SerializeField] TextMeshProUGUI txtBox;
    public string[] dialogue;
    private int index;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canClick)
        {
            if (readyToReturn && index >= dialogue.Length-1)
            {
                gameObject.SetActive(false);
                mapManager.UpdateInfo();
            }
            else if (index < dialogue.Length-1)
            {
                index++;
                txtBox.text = dialogue[index];
            }
            else
            {
                txtBox.text = "";
            }
        }
    }

    public IEnumerator NewDialogue(string[] newDialogue, bool dialogueOnly)
    {
        dialogue = newDialogue;
        index = 0;
        readyToReturn = dialogueOnly;
        if (!dialogueOnly)
        {
            
            txtBox.text = "";
            canClick = false;
            yield return new WaitForSeconds(1);
            canClick = true;
        }
        txtBox.text = dialogue[0];
        yield return null;
    }
}
