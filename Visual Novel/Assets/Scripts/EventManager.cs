using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public Event[] events;
    private CharacterManager charManager;
    private PlayerManager player;


    private void Start()
    {
        charManager = GameObject.Find("Character Manager").GetComponent<CharacterManager>();
        player = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
    }

    public Event SelectEvent(int loc, List<string> charsHere, int day, int loop)
    {
        List<Event> candidates = new List<Event>();
        foreach (Event e in events)
        {
            if (ValidEvent(e, loc, charsHere, day, loop))
            {
                candidates.Add(e);
                //Debug.Log("Adding " + e.name);
            }
        }
        if (candidates.Count == 0)
        {
            //Debug.Log("NO VALID EVENTS!!");
            return events[0];
        }
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    private bool ValidEvent(Event e, int loc, List<string> charsHere, int day, int loop)
    {
        //Debug.Log("Checking " + e.name + "...");
        //filter by already played
        if (e.played)
        {
            //Debug.Log("ALREADY PLAYED!");
            return false;
        }
        //filter by loc
        if ((e.loc != loc && e.loc != 0))
        {
            //Debug.Log("WRONG LOC (needs " + e.loc + ")");
            return false;
        }
        //filter by timing
        /*if (e.timing.Length > 0)
        {
            if (GetVec3(e.timing, day, loop) == 0)
            {
                //Debug.Log("WRONG TIMING");
                return false;
            }
        }*/
        //filter by prereqs
        foreach (string req in e.prereqsNeeded)
        {
            if (!player.prereqs.Contains(req))
            {
                //Debug.Log("MISSING PREREQ (" + req + ")");
                return false;
            }
        }
        //filter by chars at loc
        foreach (string c in charsHere)
        {
            if (Array.IndexOf(e.chars, c) == -1)
            {
                //Debug.Log("DOESN'T INCLUDE CHARACTER PRESENT (" + c + ")");
                return false;
            }
        }
        //filter by chars in event
        foreach (string character in e.chars)
        {
            foreach (Character c in charManager.characters)
            {
                if (c.name == character)
                {
                    int currentLoc = GetVec3(c.schedule, day, loop);
                    if (currentLoc != loc && currentLoc != 0)
                    {
                        //Debug.Log("NECESSARY CHARACTER IS SOMEWHERE ELSE (" + c.name + ")");
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private int GetVec3(Vector3[] vec, int day, int loop)
    {
        
        if (day == 0)
            return (int)vec[(loop-1)%vec.Length].x;
        else if (day == 1)
            return (int)vec[(loop-1)%vec.Length].y;
        else if (day == 2)
            return (int)vec[(loop-1)%vec.Length].z;
        return 0;
    }
}


[System.Serializable]
public class Event
{
    public string name;
    public string[] dialogue;
    public string[] chars;
    public int loc; //0 = anywhere, 1-4 = locations
    //public Vector3[] timing;
    public string[] prereqsNeeded;
    public string[] prereqsGained;
    public bool played;
}