using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Character[] characters;

    public void ChangeRelationship(string character, int amount)
    {
        foreach (Character c in characters)
        {
            if (c.name == character)
                c.relationship += amount;
        }
    }
}


[System.Serializable]
public class Character
{
    public string name;
    public int relationship;
    public Vector3[] schedule; //0 = undecided, 1-4 = locations
    public GameObject icon;
}