using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventPlayer : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;

    [SerializeField] private GameObject fishingGame;
    [SerializeField] private GameObject market;
    [SerializeField] private GameObject purpleSprite;
    [SerializeField] private GameObject clickToEnd;

    [HideInInspector] public bool eventStarted;
    public bool readyToReturn;
    private bool returned;

    [SerializeField] TextMeshProUGUI txtBox;
    public string[] dialogue;
    private int index;
    private bool playingLine;
    private bool skip;
    [SerializeField] private float lineDelay;
    private float lineDelayTimer;


    private List<GameObject> sprites;
    private int loc;

    private string[,] locationTxt = new string[,]{
        {"You head down to the docks this morning, ready for a day on the water.", "\"Time for some fishing!\" you say to yourself.", "The docks are quieter at night, almost peaceful. You stop for a moment to hear the waves lap against the boats."},
        {"You make your way to the market bright and early today!", "The market is bustling at this time of day, vibrant sights, smells, and sounds all assaulting your senses.", "Though the sun has set, the market is surprisingly busy in the evenings."},
        {"You head off to the bar, perhaps just <i>a bit</i> too early for responsible drinking.", "The bar is mostly empty in the middle of the afternoon, though some villagers lounge around or chow down on plates of seafood.", "You duck under the neon lit sign for the \"Mermaid's Tale\" bar, ready for a night of excitement..."},
        {"Ready for a day out in nature, you wind your way out to the edge of the sea cliffs. The sun has just risen, birds are chirping, it all seems so peaceful...", "The sun beats down as you trek out to the cliffside, glistening off the water far below.", "Away from the lights of the village, you can see countless stars twinkling above you."}
    };

    private int shopIndex;
    private string[] shopDialogue;
    private string[][] shopTxt = new string[][]{
        new string[] {"\"Hi there, what can I get for ya?\""},
        new string[] {"\"I hope you brought a good haul for me today!\"", "\"I rely on you for my supply, you know.\""},
        new string[] {"Purple seems busy behind the counter, scribbling away at a sheet of notes..."},
        new string[] {"Purple greets you with a smile as you arrive: \"Well, look who decided to turn up today!\"", "\"Tell me, what did you catch out there? Anything rare?\"", "\"Customers will go wild for something like a Pearl-Catcher, you know.\""},
        new string[] {"The stall seems empty as you walk up...", "But then Purple stands up from behind a barrel of Red Macklers, hastily wiping their hands on an apron and rushing over to the counter.", "\"Sorry, sorry, just got caught up packing up these Macklers...\"", "\"You got anything for me today?\""}
    };



    void Update()
    {
        if (!eventStarted && Input.GetMouseButtonDown(0))
        {
            eventStarted = true;
            fishingGame.SetActive(loc==0);
            market.SetActive(loc==1);
            if (loc == 1)
            {
                purpleSprite.GetComponent<RectTransform>().anchoredPosition = new Vector2(128, -8);
                purpleSprite.SetActive(true);
            }
            else    
                ShowSprites();
        }

        if (eventStarted)
        {
            if (Input.GetMouseButtonDown(0) && readyToReturn)
            {
                if (playingLine)
                    skip = true;
                else if (index < dialogue.Length-1)
                {
                    index++;
                    StartCoroutine(PlayLine(dialogue[index]));
                }
            }
            if (index >= dialogue.Length-1 && !playingLine && readyToReturn && loc != 1)
            {
                clickToEnd.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    ReturnToMap();
                }
            }

            lineDelayTimer = Mathf.Max(0, lineDelayTimer-Time.deltaTime);
            if (index < dialogue.Length-1 && !playingLine && lineDelayTimer <= 0)
            {
                index++;
                StartCoroutine(PlayLine(dialogue[index]));
            }
        }
    }

    public void SetupEvent(string[] newDialogue, int newLoc, int time, List<GameObject> newSprites)
    {
        loc = newLoc;
        if (loc == 1)
        {
            dialogue = shopTxt[Random.Range(0, shopTxt.Length)];
        }
        else
        {
            dialogue = newDialogue;
            sprites = newSprites;
        }
        index = -1;
        
        readyToReturn = (loc != 0 && loc != 1);
        txtBox.text = locationTxt[loc, time];
        returned = false;
        fishingGame.SetActive(false);
        market.SetActive(false);
        clickToEnd.SetActive(false);
    }

    private void ShowSprites()
    {
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
            else if (loc == 1)
            {
                //idk, place them outside the stall?
            }
            else
            {
                sprites[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-70*(sprites.Count-1) + 140*i, 6.5f);
            }
            sprites[i].SetActive(true);
        }
    }

    public void ReturnToMap()
    {
        if (!returned)
        {
            returned = true;
            StartCoroutine(mapManager.ShowTimeTransition());
        }
    }

    private IEnumerator PlayLine(string line)
    {
        playingLine = true;
        txtBox.text = "";
        skip = false;
        foreach (char c in line)
        {
            if (skip)
            {
                txtBox.text = line;
                break;
            }

            if (c != '*')
                txtBox.text += c;
            
            /*if (skip)
                yield return new WaitForSeconds(0.001f);
            else*/ if (c == '*')
                yield return new WaitForSeconds(0.1f);
            else if (c == '.' || c == '?' || c == '!') //add condition so quotes after punctuation appear with it
                yield return new WaitForSeconds(0.3f);
            else if (c == ',')
                yield return new WaitForSeconds(0.15f);
            else if (c == ' ')
                yield return new WaitForSeconds(0.05f);
            else
                yield return new WaitForSeconds(0.03f);
        }
        playingLine = false;
        lineDelayTimer = lineDelay;
    }
}
