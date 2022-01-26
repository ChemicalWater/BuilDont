using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBackgroundMusic : MonoBehaviour
{
    private static bool switchMusic = false;
    private static int newMusicIndex = 0;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] zoneMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    private void Update()
    {
        if(switchMusic)
        {
            switchMusic = false;
            StartCoroutine(SwitchTracks(zoneMusic[newMusicIndex]));
        }
        
    }

    public static void SwitchToMusicByIndex(int index)
    {
        switchMusic = true;
        newMusicIndex = index;
    }

    private IEnumerator SwitchTracks(AudioClip clipToSwitchTo)
    {
        animator.enabled = true;
        animator.Play("background_music");
        yield return new WaitForSeconds(2);
        source.clip = clipToSwitchTo;
        source.Play();
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        source.enabled = false;
    }
}
