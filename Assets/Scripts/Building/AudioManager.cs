using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource playAudio;
    [SerializeField] private AudioClip[] destructionSounds;//list of possible sounds to play

    // Start is called before the first frame update
    void Start()
    {
        //set the clip to a random sound
        playAudio.clip = destructionSounds[Random.Range(0, destructionSounds.Length)];
        //play the sound
        playAudio.Play();
    }
}
