using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventPlayer : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;

    [SerializeField] private GameObject fishingGame;
    [SerializeField] private GameObject market;

    private bool canClick = true;
    public bool readyToReturn;

    [SerializeField] TextMeshProUGUI txtBox;
    public string[] dialogue;
    private int index;
    private List<GameObject> sprites;
    private int loc;

    private string[,] locationTxt = new string[,]{
        {"You head down to the docks this morning, ready for a day on the water.", "\"Time for some fishing!\" you say to yourself.", "The docks are quieter at night, almost peaceful. You stop for a moment to hear the waves lap against the boats."},
        {"You make your way to the market bright and early today!", "The market is bustling at this time of day, vibrant sights, smells, and sounds all assaulting your senses.", "Though the sun has set, the market is surprisingly busy in the evenings."},
        {"You head off to the bar, perhaps just <i>a bit</i> too early for responsible drinking.", "The bar is mostly empty in the middle of the afternoon, though some villagers lounge around or chow down on plates of seafood.", "You duck under the neon lit sign for the \"Mermaid's Tale\" bar, ready for a night of excitement..."},
        {"Ready for a day out in nature, you wind your way out to the edge of the sea cliffs. The sun has just risen, birds are chirping, it all seems so peaceful...", "The sun beats down as you trek out to the cliffside, glistening off the water far below.", "Away from the lights of the village, you can see countless stars twinkling above you."}
    };



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
                if (index == 0)
                {
                    ShowEvent();
                }
            }
            else
            {
                txtBox.text = "";
            }
        }
    }

    public void SetupEvent(string[] newDialogue, int newLoc, int time, List<GameObject> newSprites, bool dialogueOnly)
    {
        dialogue = newDialogue;
        index = -1;
        sprites = newSprites;
        loc = newLoc;
        txtBox.text = locationTxt[loc, time];
        readyToReturn = dialogueOnly;
        fishingGame.SetActive(false);
        market.SetActive(false);
    }

    private void ShowEvent()
    {
        fishingGame.SetActive(loc==0);
        market.SetActive(loc==1);

        //TODO: have sprites come in at a more dynamic time
        for (int i = 0; i < sprites.Count; i++)
        {
            if (loc == 0)
            {
                int[] spriteX = new int[]{-170, 170, -270};
                if (sprites.Count > 2)
                    spriteX[0] = -140;
                sprites[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(spriteX[i], 6.5f);
            }
            else
            {
                sprites[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-70*(sprites.Count-1) + 140*i, 6.5f);
            }
            sprites[i].SetActive(true);
        }
    }

}
