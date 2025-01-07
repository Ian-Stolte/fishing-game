using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public Character[] characters;
    [SerializeField] private GameObject hearts;
    [SerializeField] private TMPro.TextMeshProUGUI heartsTitle;


    public IEnumerator ChangeRelationship(string name, int amount)
    {
        Character c = characters[0];
        hearts.GetComponent<Animator>().Play("HeartsSlideIn");
        heartsTitle.text = name;
        foreach (Character ch in characters)
        {
            if (ch.name == name)
                c = ch;
        }
        for (int i = 0; i < 10; i++)
        {
            hearts.transform.GetChild(2+i).GetComponent<Image>().enabled = i <= c.relationship;
        }
        yield return new WaitForSeconds(1.5f);
        c.relationship += amount;
        if (amount > 0)
        {
            hearts.transform.GetChild(2+c.relationship).GetComponent<Image>().enabled = true;
            GameObject sparkle = hearts.transform.GetChild(2+c.relationship).GetChild(2).gameObject;
            sparkle.SetActive(true);
            for (float i = 0; i < 1; i += 0.01f)
            {
                sparkle.GetComponent<CanvasGroup>().alpha = 1-i;
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            hearts.transform.GetChild(c.relationship).GetComponent<Image>().enabled = false;
            yield return new WaitForSeconds(1);
        }
        hearts.GetComponent<Animator>().Play("HeartsSlideOut");
    }
}


[System.Serializable]
public class Character
{
    public string name;
    public int relationship;
}