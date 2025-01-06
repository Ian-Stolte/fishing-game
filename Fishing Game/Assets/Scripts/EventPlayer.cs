using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventPlayer : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;

    [SerializeField] private GameObject fishingGame;
    [SerializeField] private GameObject market;
    [SerializeField] private GameObject cooking;
    [SerializeField] private GameObject foraging;

    [SerializeField] private GameObject violetSprite;
    [SerializeField] private GameObject clickToEnd;
    [SerializeField] private Transform spriteParent;
    [SerializeField] private Transform portraitParent;
    [SerializeField] private GameObject clickButton;

    [HideInInspector] public bool eventStarted;
    public bool readyToReturn;
    private bool returned;

    public Event currentEvent;
    private List<GameObject> sprites;
    private int loc;

    [SerializeField] private TextMeshProUGUI checkPopup;
    [SerializeField] private TextMeshProUGUI abilityUpdate;
    [SerializeField] private Color failedColor;

    [SerializeField] TextMeshProUGUI txtBox;
    public string[] dialogue;
    [SerializeField] private int index;
    private bool playingLine;
    private bool skip;
    [SerializeField] private float lineDelay;
    private float lineDelayTimer;

    [SerializeField] private Transform choices;
    private bool choosing;

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
        new string[] {"Violet seems busy behind the counter, scribbling away at a sheet of notes..."},
        new string[] {"Violet greets you with a smile as you arrive: \"Well, look who decided to turn up today!\"", "\"Tell me, what did you catch out there? Anything rare?\"", "\"Customers will go wild for something like a Pearl-Catcher, you know.\""},
        new string[] {"The stall seems empty as you walk up...", "But then Violet stands up from behind a barrel of Red Macklers, hastily wiping her hands on an apron and rushing over to the counter.", "\"Sorry, sorry, just got caught up packing up these Macklers...\"", "\"You got anything for me today?\""}
    };

    private PlayerManager player;
    private FishTracker fishTracker;


    void Awake()
    {
        player = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
        fishTracker = GameObject.Find("Fish Tracker").GetComponent<FishTracker>();
    }


    void Update()
    {
        if (!returned && !choosing)
        {
            /*else if (choosing)
            {
                //arrow keys to mouse between options?
            }*/

            /*if (Input.GetMouseButtonDown(0) && readyToReturn)
            {
                if (playingLine)
                    skip = true;
                else if (index < dialogue.Length-1)
                {
                    index++;
                    StartCoroutine(PlayLine(dialogue[index]));
                }
            }*/
            if (index >= dialogue.Length-1 && !playingLine && (readyToReturn || loc != 0))
            {
                clickToEnd.SetActive(true);
            }

            lineDelayTimer = Mathf.Max(0, lineDelayTimer-Time.deltaTime);
            if (index < dialogue.Length-1 && !playingLine && lineDelayTimer <= 0)
            {
                index++;
                StartCoroutine(PlayLine(dialogue[index]));
            }
        }
    }


    public void ClickToAdvance()
    {
        if (playingLine)
            skip = true;
        else if (index < dialogue.Length-1)
        {
            index++;
            StartCoroutine(PlayLine(dialogue[index]));
        }
        else if (index >= dialogue.Length-1 && !playingLine && (readyToReturn || loc != 0))
        {
            ReturnToMap();
        }
    }


    public void SetupEvent(Event e, int newLoc, int time)
    {
        loc = newLoc;
        currentEvent = e;
        sprites = new List<GameObject>();
        /*if (loc == 1)
            dialogue = shopTxt[Random.Range(0, shopTxt.Length)];
        else*/
            dialogue = e.dialogue;
        index = 0;

        returned = false;
        readyToReturn = true;
        fishingGame.SetActive(false);
        market.SetActive(false);
        cooking.SetActive(false);
        foraging.SetActive(false);
        clickToEnd.SetActive(false);
        ShowPortrait("none");
        clickButton.SetActive(true);
        StartCoroutine(PlayLine(dialogue[index]));
    }


    private void ShowSprites(string spriteName, bool add=true)
    {
        if (spriteName == "Violet" && loc == 1)
            spriteName = "Violet (Market)";
        foreach (Transform child in spriteParent)
        {
            if (child.name == spriteName)
            {
                if (add)
                    sprites.Add(child.gameObject);
                else
                {
                    sprites.Remove(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }
        for (int i = 0; i < sprites.Count; i++)
        {
            if (loc == 0)
            {
                int[] spriteX = new int[]{-160, 170, -270};
                if (sprites.Count > 2)
                    spriteX[0] = -140;
                sprites[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(spriteX[i], 6.5f);
            }
            /*else if (loc == 1)
            {
                //idk, place them outside the stall?
            }*/
            else
            {
                if (sprites.Count == 1)
                    sprites[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 6.5f);
                else if (sprites.Count == 2)
                    sprites[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-100*(sprites.Count-1) + 200*i, 6.5f);
                else if (sprites.Count == 3)
                    sprites[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-80*(sprites.Count-1) + 160*i, 6.5f);
            }
            if (sprites[i].name == "Violet (Market)" && market.activeSelf)
                sprites[i].SetActive(false);
            else
                sprites[i].SetActive(true);
        }
    }


    public void ReturnToMap()
    {
        if (!returned)
        {
            returned = true;
            GameObject.Find("Event Manager").GetComponent<EventManager>().RemoveEvents(currentEvent);
            StartCoroutine(mapManager.ShowTimeTransition());
        }
    }


    public void MakeChoice(int i)
    {
        choosing = false;
        txtBox.transform.parent.gameObject.SetActive(true);
        foreach (Transform child in choices)
            child.gameObject.SetActive(false);
        
        while (currentEvent.speakers[index] != ("Result " + i))
        {
            index++;
        }
        index++;
        StartCoroutine(PlayLine(dialogue[index]));
    }


    private IEnumerator PlayLine(string line, bool locIntro=false)
    {
        //Debug.Log("Called with speaker=" + currentEvent.speakers[index] + " and line=" + line + " (locIntro=" + locIntro + ")");
        //player choice
        if (currentEvent.speakers[index] == "Loc Intro" && !locIntro)
        {
            StartCoroutine(PlayLine(locationTxt[loc, mapManager.time], true));
        }
        else if (currentEvent.speakers[index] == "Show Loc")
        {
            fishingGame.SetActive(loc==0);
            if (loc == 1)
            {
                market.SetActive(true);
                foreach (Transform child in spriteParent)
                    if (child.name == "Violet (Market)")
                        child.gameObject.SetActive(false);
            }
            cooking.SetActive(loc==2);
            foraging.SetActive(loc==3);
            readyToReturn = (loc==3);
            txtBox.text = "";
            if (index < dialogue.Length-1)
            {
                index++;
                StartCoroutine(PlayLine(dialogue[index]));
            }
        }
        else if (currentEvent.speakers[index] == "Option 1")
        {
            txtBox.transform.parent.gameObject.SetActive(false);
            int numOptions = 0;
            while (currentEvent.speakers[index+numOptions].Contains("Option"))
            {
                numOptions++;
            }
            for (int i = 0; i < numOptions; i++)
            {
                choices.GetChild(i).gameObject.SetActive(true);
                if (numOptions==1)
                    choices.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -124);
                else if (numOptions==2)
                    choices.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -62 - 73*i); //-63, -135
                else
                    choices.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -37 - 73*i); //-37, -110, -183
                choices.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = dialogue[index+i];
            }
            choosing = true;
            yield return null;
        }
        else if (currentEvent.speakers[index] == "Prereq")
        {
            string[] splitStr = line.Split('[');
            if (splitStr.Length > 1) //if delayed prereq
            {
                string[] splitAgain = splitStr[1].Split(',');
                if (splitAgain.Length > 1) //if calendar event
                {
                    mapManager.AddToCalendar(splitAgain[0], int.Parse(splitAgain[1].Substring(1, splitAgain[1].Length-2)));
                    int reqTime = int.Parse(splitAgain[1].Substring(0, splitAgain[1].Length-1)) + mapManager.time + mapManager.day*3;
                    player.delayedPrereqs.Add(splitStr[0].Trim(), reqTime);
                }
                else
                {
                    int reqTime = int.Parse(splitStr[1].Substring(0, splitStr[1].Length-1)) + mapManager.time + mapManager.day*3;
                    player.delayedPrereqs.Add(splitStr[0].Trim(), reqTime);
                }
            }
            else
                player.prereqs.Add(splitStr[0]);
            index++;
            if (index < dialogue.Length)
                StartCoroutine(PlayLine(dialogue[index]));
        }
        else if (currentEvent.speakers[index] == "Jump")
        {
            index = int.Parse(line);
            StartCoroutine(PlayLine(dialogue[index]));
        }
        else if (currentEvent.speakers[index].Contains("Result"))
        {
            while (index < currentEvent.speakers.Length-1)
            {
                index++;
                if (currentEvent.speakers[index] == "Merge")
                {
                    index++;
                    if (index < dialogue.Length)
                        StartCoroutine(PlayLine(dialogue[index]));
                    break;
                }
            }
        }
        else if (currentEvent.speakers[index] == "Merge")
        {
            index++;
            StartCoroutine(PlayLine(dialogue[index]));
        }
        else if (currentEvent.speakers[index] == "Stats")
        {
            string[] splitStr = currentEvent.dialogue[index].Split('[');
            string amountStr = splitStr[1].Substring(0, splitStr[1].Length-1);
            if (amountStr.Contains("*"))
            {
                string floatStr = amountStr.Substring(1, amountStr.Length-1);
                player.MultiplyStats(splitStr[0].Trim(), float.Parse(floatStr));
                float percent = (float.Parse(floatStr)-1)*100;
                if (percent > 0)
                {
                    abilityUpdate.text = "+" + percent + "% " + splitStr[0].Trim().ToUpper() + "!";
                    abilityUpdate.color = player.StatColor(splitStr[0].Trim());
                }
                else
                {
                    abilityUpdate.text = percent + "% " + splitStr[0].Trim().ToUpper();
                    abilityUpdate.color = failedColor;
                }
                abilityUpdate.GetComponent<Animator>().Play("AbilityUpdate");
            }
            else
            {
                int amount = int.Parse(amountStr);

                if (splitStr[0].Contains("Relationship")) //e.g Relationship-Rein [+1]
                {
                    string character = splitStr[0].Substring(13, splitStr[0].Length-14);
                    GameObject.Find("Character Manager").GetComponent<CharacterManager>().ChangeRelationship(character, amount);
                }
                else //e.g Arts [-2]
                {
                    player.AddStats(splitStr[0].Trim(), amount);
                    if (amount > 0)
                    {
                        abilityUpdate.text = "+" + amount + " " + splitStr[0].Trim().ToUpper() + "!";
                        abilityUpdate.color = player.StatColor(splitStr[0].Trim());
                    }
                    else
                    {
                        abilityUpdate.text = amount + " " + splitStr[0].Trim().ToUpper();
                        abilityUpdate.color = failedColor;
                    }
                    abilityUpdate.GetComponent<Animator>().Play("AbilityUpdate");
                }
            }
            index++;
            StartCoroutine(PlayLine(dialogue[index]));
        }
        else if (currentEvent.speakers[index] == "Stat Check")
        {
            if (dialogue[index].Contains(">"))
            {
                string[] splitStr = dialogue[index].Split('>');
                if (player.StrToStat(splitStr[0].Trim()) > int.Parse(splitStr[1].Trim()))
                {
                    StartCoroutine(StatPopup(splitStr[0].Trim(), true));
                }
                else
                {
                    StartCoroutine(StatPopup(splitStr[0].Trim(), false));
                    while (currentEvent.speakers[index] != "Merge" && currentEvent.speakers[index] != "Jump" && index < currentEvent.speakers.Length-1)
                        index++;
                }
            }
            else if (dialogue[index].Contains("<"))
            {
                string[] splitStr = dialogue[index].Split('<');
                if (player.StrToStat(splitStr[0].Trim()) < int.Parse(splitStr[1].Trim()))
                {
                    //StartCoroutine(StatPopup(splitStr[0].Trim(), false));
                }
                else
                {
                    //StartCoroutine(StatPopup(splitStr[0].Trim(), true));  // <--- maybe not? Like for money < 3 don't want to see a popup... (maybe have a different code for hidden check)
                    while (currentEvent.speakers[index] != "Merge" && currentEvent.speakers[index] != "Jump" && index < currentEvent.speakers.Length-1)
                        index++;
                }
            }
            index++;
            if (index < dialogue.Length)
                StartCoroutine(PlayLine(dialogue[index]));
        }
        else
        {
            line = line.Replace("{name}", player.name);
            line = line.Replace("{loc}", mapManager.locations[loc].name.ToLower());
            line = line.Replace("{fish}", fishTracker.fish[Random.Range(0, fishTracker.fish.Length)].name);
            line = line.Replace("{they}", player.pronouns[0]);
            line = line.Replace("{them}", player.pronouns[1]);
            line = line.Replace("{their}", player.pronouns[2]);
            if (line.Contains("{time"))
            {
                int startIndex = 0;
                while (line.IndexOf("{time", startIndex) != -1)
                {
                    startIndex = line.IndexOf("{time", startIndex);
                    int endIndex = line.IndexOf('}', startIndex);
                    if (endIndex - startIndex == 5)
                    {
                        line = line.Replace(line.Substring(startIndex, endIndex-startIndex+1), mapManager.TimeString());
                    }
                    else
                    {
                        int n = int.Parse(line.Substring(endIndex-1, 1));
                        string moddedTime = mapManager.ReturnModdedTime(n);
                        line = line.Replace(line.Substring(startIndex, endIndex-startIndex+1), moddedTime);
                        startIndex += moddedTime.Length;
                    }
                }
            }
            playingLine = true;
            txtBox.text = "";
            skip = false;
            if (currentEvent.sprites[index] != "")
            {
                string[] spritesToMod = currentEvent.sprites[index].Split(new string[] { ", " }, System.StringSplitOptions.None);
                foreach (string str in spritesToMod)
                {
                    if (str.Contains("- "))
                    {
                        ShowSprites(str.Substring(2), false);
                    }
                    else
                        ShowSprites(str);
                }
            }
            ShowPortrait(currentEvent.speakers[index]);
            
            bool addingHTML = false;
            string HTMLtag = "";
            foreach (char c in line)
            {
                if (c == '<')
                {
                    addingHTML = true;
                    HTMLtag = "";
                }
                else if (c == '>')
                {
                    addingHTML = false;
                    txtBox.text += (HTMLtag + c);
                    continue;
                }

                if (addingHTML)
                    HTMLtag += c;
                else if (c != '*')
                    txtBox.text += c;
                
                if (!skip && !addingHTML)
                {
                    if (c == '*')
                        yield return new WaitForSeconds(0.1f);
                    else if (c == '.' || c == '?' || c == '!' || c == ':' || c == '—') //add condition so quotes after punctuation appear with it //also speed up ellipses
                        yield return new WaitForSeconds(0.3f);
                    else if (c == ',' || c == ';')
                        yield return new WaitForSeconds(0.15f);
                    else if (c == ' ')
                        yield return new WaitForSeconds(0.05f);
                    else
                        yield return new WaitForSeconds(0.03f);
                }
            }
            playingLine = false;
            lineDelayTimer = lineDelay;
        }
    }


    private IEnumerator StatPopup(string stat, bool success)
    {
        checkPopup.gameObject.SetActive(true);
        if (success)
        {
            checkPopup.text = stat + " succeeded!";
            checkPopup.color = player.StatColor(stat.Trim());
        }
        else
        {
            checkPopup.text = stat + " failed...";
            checkPopup.color = failedColor;
        }
        yield return new WaitForSeconds(2);
        for (float i = 1; i > 0; i -= 0.02f)
        {
            checkPopup.color = new Color(checkPopup.color.r, checkPopup.color.g, checkPopup.color.b, i);
            yield return new WaitForSeconds(0.01f);
        }
        checkPopup.gameObject.SetActive(false);
    }


    private void ShowPortrait(string name)
    {
        foreach (Transform child in portraitParent)
        {
            child.gameObject.SetActive(child.name == name);
        }
        if (name == "Violet" && loc == 1)
            name = "Violet (Market)";
        foreach (GameObject g in sprites)
        {
            if (g.name == name)
                g.GetComponent<CanvasGroup>().alpha = 1;
            else
                g.GetComponent<CanvasGroup>().alpha = 0.6f;
        }
    }
}