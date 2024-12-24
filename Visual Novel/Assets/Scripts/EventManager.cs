using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EventManager : MonoBehaviour
{
    public Event[] events;
    public List<Event> dockEvents;
    public List<Event> marketEvents;

    private CharacterManager charManager;
    private PlayerManager player;


    private void Start()
    {
        charManager = GameObject.Find("Character Manager").GetComponent<CharacterManager>();
        player = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
        
        LoadFromJson(dockEvents, "Resources/Events/Docks");
        LoadFromJson(marketEvents, "Resources/Events/Market");
    }
    
    private void LoadFromJson(List<Event> destination, string path)
    {
        string fullPath = Path.Combine(Application.dataPath, path);

        if (Directory.Exists(fullPath))
        {
            string[] files = Directory.GetFiles(fullPath);
            foreach (string file in files)
            {
                if (!file.Contains(".meta"))
                {
                    string relativePath = Path.GetRelativePath(Application.dataPath, file).Substring("Resources".Length+1);
                    relativePath = Path.ChangeExtension(relativePath, null);
                    var res = Resources.Load<TextAsset>(relativePath).text;
                    var txt = ParseJsonToDictionary(res);

                    Event e = new Event(Path.ChangeExtension(Path.GetFileName(file), null));
                    e.speakers = txt["Speaker"].Values.ToArray();
                    e.dialogue = txt["Dialogue"].Values.ToArray();
                    e.chars = txt["Chars"].Values.Where(value => !string.IsNullOrEmpty(value)).ToArray();
                    e.prereqsNeeded = txt["Prereqs-Needed"].Values.Where(value => !string.IsNullOrEmpty(value)).ToArray();
                    e.prereqsGained = txt["Prereqs-Gained"].Values.Where(value => !string.IsNullOrEmpty(value)).ToArray();
                    
                    destination.Add(e);
                }
            }
        }
    }

    private Dictionary<string, Dictionary<string, string>> ParseJsonToDictionary(string jsonString)
    {
        try
        {
            var parsedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonString);
            return parsedData;
        }
        catch (JsonException ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
            return null;
        }
    }


    public Event SelectEvent(int loc, List<string> charsHere, int time, int day)
    {
        List<Event> candidates = new List<Event>();
        List<Event> locEvents = dockEvents;
        if (loc == 2)
            locEvents = marketEvents;
        
        foreach (Event e in locEvents)
        {
            if (ValidEvent(e, loc, charsHere, time, day))
            {
                candidates.Add(e);
            }
        }
        if (candidates.Count == 0)
        {
            return events[0];
        }
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }


    private bool ValidEvent(Event e, int loc, List<string> charsHere, int time, int day)
    {
        //Debug.Log("Checking " + e.name + "...");
        //filter by already played
        if (e.played)
        {
            //Debug.Log("ALREADY PLAYED!");
            return false;
        }
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
            if (e.chars.Contains(c))
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
                    int currentLoc = GetVec3(c.schedule, time, day);
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

    private int GetVec3(Vector3[] vec, int time, int day)
    {
        
        if (time == 0)
            return (int)vec[(day-1)%vec.Length].x;
        else if (time == 1)
            return (int)vec[(day-1)%vec.Length].y;
        else if (time == 2)
            return (int)vec[(day-1)%vec.Length].z;
        return 0;
    }
}


[System.Serializable]
public class Event
{
    public Event(string name_)
    {
        name = name_;
    }

    public string name;
    public string[] speakers;
    public string[] dialogue;
    public string[] chars;
    //public Vector3[] timing;

    public string[] prereqsNeeded;
    public string[] prereqsGained;
    public bool played;
}