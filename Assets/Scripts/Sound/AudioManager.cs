using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    /*
    This script manages all sounds for the project. It is an adaptation of the unity sound tutorial by Brackys, 
    found here: 
    https://www.youtube.com/watch?v=6OT43pvUyfY 
    */
    public sound[] sounds;
    private float orangeGellVol = 0f;
    private float orangeGellVolSpeed = 0.005f;
    private bool timerActive = false;
    private sound orangegell;

    void Awake() 
    {
        // construct an AudioSource for each sound listed in the global sound[] array
        foreach (sound s in sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        orangegell = Array.Find(sounds, sound => sound.name == "orangegell");
    }

    // simple play sound function
    public void Play(string name) 
    {
        sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();    
    }

    // stop playing a sound
    public void Stop(string name) 
    {
        sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();    
    }

    // used to fade in a sound by increasing the volume by activating the timer, 
    // setting the volume to 0 and playing the sound. Fade volume handled in Update()
    public void orangeFadeIn(string name) 
    {
        if (!timerActive) 
        {
            timerActive = true;
            if (orangeGellVol < 0.01f) 
            {
                orangegell.source.volume = 0;
                orangegell.source.Play();
            }
        }
    }

    // stops the timer to fade out the sound
    public void orangeFadeOut(string name) 
    {
        timerActive = false;
    }

    void Update() 
    {
        if (timerActive & orangeGellVol < 1) 
        {
            // if we need to fade in the sound, increase the volume by orangeGellVolSpeed each time frame
            orangeGellVol += orangeGellVolSpeed;
        }
        if(!timerActive && orangeGellVol > 0) {
            // if we need to fade out the sound, decrease the volume by orangeGellVolSpeed each time frame
            orangeGellVol -= orangeGellVolSpeed;
        }
        // apply volume changes
        orangegell.source.volume = orangeGellVol;
    }
}
