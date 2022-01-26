//Code by Eele Roet
using TMPro;
using TurnTheGameOn.Timer;
using UnityEngine;

/// <summary>
/// <para>This script controls the timer that keeps track of mission time.</para>
/// <para>Toggles the timer, sets the time, changes to red when time gets low</para>
/// </summary>
public class MissionTimerScript : MonoBehaviour
{
    [SerializeField] private AudioSource source;//audio source for playing "time almost up" sound
    [SerializeField] private AudioClip hurryUpSound;//time almost up sound
    [SerializeField] private TMP_Text timerText;//the text element that will be the timer
    [SerializeField] private Timer timer;//the timer asset in the scene


    //event method that is called by the timer asset
    public void HurryUp()
    {
        source.clip = hurryUpSound;//sets the clip to the right sound
        source.Play();

        timer.displayOptions.milliseconds = true;//displays the milliseconds
        timerText.color = Color.red;//shows the time in red
    }

    //public method for starting the timer
    public void StartTimer(float time)
    {
        this.gameObject.SetActive(true);//enables the timer

        
        timer.startTime = time;//sets the start time
        timer.ResetTimer();//resets the timer so all events will be called
        timer.displayOptions.milliseconds = false;//starts with only displaying minutes and seconds
        timer.timerState = TimerState.Counting;//timer is now running

        timerText.color = Color.white;//resets the text color
    }
}
