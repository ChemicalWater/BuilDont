using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicOnTriggerScript : MonoBehaviour
{
    [SerializeField] private int backgroundMusicIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayBackgroundMusic.SwitchToMusicByIndex(backgroundMusicIndex);
        }
    }
}
