//Author: Ian Stolte
//Date: 7/13/23
//Desc: Manages the game audio (sound effects are called from other scripts, level music is called here)

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Editable")]
    public Sound[] music;
    public Sound[] sfx;

    [Header("Don't edit")]
    public AudioSource[] audios;

    public Sound currentSong;

    void Awake()
    {
        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        foreach (Sound s in sfx)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        audios = gameObject.GetComponents<AudioSource>();
    }

    void Start()
    {
        foreach (Sound s in music)
        {
            float storedVol = s.volume;
            s.volume = 0;
            s.source.Play();
            s.source.Stop();
            s.volume = storedVol;
        }

        Play("Uncharted Waters");
    }
    
    public IEnumerator FadeOutAll(float duration)
    {
        List<string> songsToFade = new List<string>();
        foreach (Sound s in music)
        {
            if (s.source.volume != 0)
            {
                StartCoroutine(StartFade(s.name, duration, 0));
                songsToFade.Add(s.name);
            }
        }
        yield return new WaitForSeconds(duration);
        foreach (Sound s in music)
        {
            if (songsToFade.Contains(s.name))
                s.source.Stop();
        }
    }

    public IEnumerator QuietAll(float duration, float n)
    {
        foreach (Sound s in music)
        {
            if (s.source.volume != 0)
                StartCoroutine(StartFade(s.name, duration, s.source.volume*n));
        }
        yield return new WaitForSeconds(duration);
        foreach (Sound s in music)
        {
            if (s.source.volume != 0)
                StartCoroutine(StartFade(s.name, duration, s.source.volume/n));
        }

    }

    public void Play(string name)
    {
        Sound s = Array.Find(sfx, sound => sound.name == name);
        if (s == null)
        {
            s = Array.Find(music, sound => sound.name == name);
            if (s != null)
                currentSong = s;
        }
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        if (s.source.volume == 0)
            s.source.volume = 0.6f;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sfx, sound => sound.name == name);
        if (s == null)
            s = Array.Find(music, sound => sound.name == name);        
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public IEnumerator StartFade(string name, float duration, float end)
    {
        Sound s = Array.Find(sfx, sound => sound.name == name);
        if (s == null)
            s = Array.Find(music, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            yield break;
        }

        float currentTime = 0;
        float start = s.source.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(start, end, currentTime / duration);
            yield return null;
        }

        //if (end == 0)
        //    s.source.Stop();
    }
}