
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sounds
    {
        public string name;         // Name to identify the sound
        public AudioClip clip;      // The actual audio clip
        [Range(0f, 1f)]
        public float volume = 0.5f; // Volume control for the sound
        [HideInInspector]
        public AudioSource source;  // The AudioSource that will play the sound
    }
    
    public Sounds[] sounds;

    public void Play(string name, bool loop = false)
    {
        Sounds currentSound = Array.Find(sounds, sound => sound.name == name);
        if(currentSound != null)
        {
            if(currentSound.source == null)
            {
                currentSound.source = gameObject.AddComponent<AudioSource>();
                currentSound.source.clip = currentSound.clip;

                currentSound.source.volume = currentSound.volume;
            }

            if (loop)
            {
                currentSound.source.loop = true;
            }
            currentSound.source.Play();
        }
    }
    public void Stop(string name)
    {
        Sounds currentSound = Array.Find(sounds, sound => sound.name == name);
        if (currentSound != null)
        {
            currentSound.source.Stop();
        }
    }

}
