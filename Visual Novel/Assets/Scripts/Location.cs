using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location
{
    public string name;
    public RectTransform iconPos;
    //public CharacterPct[] characterPcts;
    public List<string> charsHere;
}

[System.Serializable]
public class CharacterPct
{
    public float orange;
    public float green;
    public float purple;
}