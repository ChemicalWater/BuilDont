using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class bankScript : MonoBehaviour
{

    [SerializeField]
    private int lockdowntime = 30;

    [SerializeField]
    private int spawncops = 10;
    [SerializeField]
    private int timeBetweenCops = 2;
    [SerializeField]
    private int copLifetime = 30;

    private bool spawningCops;

    [SerializeField]
    private float lightBlinkingSpeed = 1.5f;

    [SerializeField] AudioSource alarmSoundLoop;
    [SerializeField] AudioSource alarmSoundEnd;

    [SerializeField]
    private GameObject lockdownMode;

    private GameObject alarmLights;

    [SerializeField] Transform spawnerPos;
    [SerializeField] GameObject copPrefab;

    [SerializeField] private GameObject bankSign;
    private float timeLeft;
    private TextMeshPro bankText;

    void Start()
    {
        // turn off lockdownmode gameobject so the bank is not instant lockdown
        lockdownMode.SetActive(false);
        // Get the alarmlights from lockdownmode
        alarmLights = lockdownMode.transform.GetChild(0).gameObject;
        // turn this to true so we can turn it off later to spawn cops only once
        spawningCops = true;
        // Get TextMeshPro from banksign gameobject so we can put some text on it
        bankText = bankSign.GetComponent<TextMeshPro>();
        // put timer on same time as lockdowntime, so the timer is in sync
        timeLeft = lockdowntime;
        // Change the text to normal so the timer is not instantly played
        bankText.text = "BANK";
    }

    public void Update()
    {
        // Check if the lockdown gameobject is active, start timer and put the text to timer if so
        if (lockdownMode.activeSelf)
        {
            timeLeft = timeLeft - Time.deltaTime;
            bankText.text = Mathf.RoundToInt(timeLeft).ToString();
        }
    }

    private IEnumerator blinkingLights()
    {
        // while the lockdown is active and the lights are found
        while (lockdownMode.activeSelf) {
            if (alarmLights != null)
            {
                // if the lights are on, turn them off
                if (alarmLights.activeSelf)
                    alarmLights.SetActive(false);
                // if the lights are off, turn them on
                else
                    alarmLights.SetActive(true);
            }
            // wait for few sec so that the lights can blink, decided by the lightblinkSpeed at top
            yield return new WaitForSeconds(lightBlinkingSpeed);
        }
    }

    public IEnumerator lockDown()
    {
        // turn bool lockdown to true in the attached buildingscript, so we can no longer destroy this building
        gameObject.GetComponent<BuildingScript>().banklockdownStart = true;
        //Debug.Log("WHOOOH LOCKDOWN LOCKDOWN");
        lockdownMode.SetActive(true);
        // Start the lights blinking, start the alarm going off and start spawning some cops
        StartCoroutine("blinkingLights");
        StartCoroutine("alarmSound");
        SpawnCops();

        // wait for set lockdown time before continuing
        yield return new WaitForSeconds(lockdowntime);

        // lockdown has ended so we tell this to attached buildingscript
        gameObject.GetComponent<BuildingScript>().bankLockdownEnd = true;
        lockdownMode.SetActive(false);
        // Make sure the cops stopped spawning by destroying their spawnpoint
        Destroy(transform.GetChild(3).gameObject);
        // change text back from timer to bank
        bankText.text = "BANK";
        // The lockdown is now actually over so we turn this back to false, the bank can now be destroyed
        gameObject.GetComponent<BuildingScript>().banklockdownStart = false;
        //Debug.Log("LETS ROB THIS PLACE");
    }

    private void SpawnCops()
    {
        if (spawningCops)
        {
            // Spawn cops with the info from above
            transform.GetChild(3).GetComponent<CopSpawner>().spawnCop(timeBetweenCops, spawncops, spawnerPos, copLifetime, copPrefab);
            // turn spawning off so we only call this once
            spawningCops = false;
        }
    }

    // Alarm sound is made so you can adjust the lockdown time and the sound still remains the same
    private IEnumerator alarmSound()
    {
        // Get the lockdown time and get rid of the last 2 sec for an end sound
        float EndTime = lockdowntime - 2.0f;
        // start looping the alarm sound
        alarmSoundLoop.Play();
        // Wait for the time we want our alarmtime to loop
        yield return new WaitForSeconds(EndTime);
        // Stop the loop
        alarmSoundLoop.Stop();
        // play our alarm ending sound
        alarmSoundEnd.Play();
    }
}
