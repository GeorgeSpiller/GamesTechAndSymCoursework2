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

    All sounds played are take from the royalty free Youtube audio libaray:  https://studio.youtube.com/channel/UCEW20lPHCioiUkflf0sXH9w/music :
    Your use of this music library (including the music files in this library) is subject to the YouTube Terms of Service. Music from this library is intended solely for use by you in videos and other content that you create. You may use music files from this library in videos that you monetize on YouTube.
    By downloading music from this library, you agree with the following:
        You may not make available, distribute or perform the music files from this library separately from videos and other content into which you have incorporated these music files (e.g., standalone distribution of these files is not permitted).
        You may not use music files from this library in an illegal manner or in connection with any illegal content.
    You agree to comply with these requirements when you use music from this library.
    Youtube Terms of service: https://www.youtube.com/t/terms
    Learn More: https://support.google.com/youtube/answer/3376882?hl=en

    */
    public sound[] sounds;
    public string[] music;

    [Range(0, 1f)]
    public float musicVolume = 0.5f;
    private float orangeGellVol = 0f;
    private float orangeGellVolSpeed = 0.005f;
    private bool timerActive = false;
    private sound orangegell;
    private int songIndex = 0;
    private string currentSong;

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
                orangegell.source.loop = true;
                if(!orangegell.source.isPlaying) 
                {
                    orangegell.source.Play();
                }
            }
        }
    }

    // stops the timer to fade out the sound
    public void orangeFadeOut(string name) 
    {
        timerActive = false;
    }

    // returns true if the song with the name is playing
    private bool isSongPlaying(string name) 
    {
        sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source.isPlaying;
    }

    private string[] Shuffle(string[] a)
	{
        // Function code written by @jasonmarziani taken from their GitHub
        // https://gist.github.com/jasonmarziani/7b4769673d0b593457609b392536e9f9
		// Loops through array
		for (int i = a.Length-1; i > 0; i--)
		{
			// Randomize a number between 0 and i (so that the range decreases each time)
			int rnd = UnityEngine.Random.Range(0,i);
			
			// Save the value of the current i, otherwise it'll overright when we swap the values
			string temp = a[i];
			
			// Swap the new and old values
			a[i] = a[rnd];
			a[rnd] = temp;
		}
		
		return a;
	}

    void Start() 
    {
        // shuffle the order of the music, and start playing it in sequence
        music = Shuffle(music);
        currentSong = music[0];
        print("Playing song: " + currentSong);
        Play(music[0]);
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

        // if song finishes, play next song
        if (!isSongPlaying(currentSong)) 
        {
            if (songIndex > music.Length) 
            {
                songIndex = 0;
            } else {
                songIndex ++;         
            }
            currentSong = music[songIndex];
            Play(music[songIndex]);
            print("Now Playing song: " + music[songIndex] );
        }
    }
}
