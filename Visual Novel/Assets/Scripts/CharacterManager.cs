using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Character[] characters;
}


[System.Serializable]
public class Character
{
    public string name;
    public int relationship;
    public Vector3[] schedule; //0 = undecided, 1-4 = locations
    public GameObject icon;
}