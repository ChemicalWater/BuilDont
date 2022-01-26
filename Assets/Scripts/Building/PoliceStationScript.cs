using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceStationScript : MonoBehaviour
{
    [SerializeField] private Transform copSpawnPoint;
    [SerializeField] private GameObject copPrefab;
    [SerializeField] private int timeBetweenSpawns;
    [SerializeField] private int maxCops;
    [SerializeField] private int copLifeTime;
    private bool called = true;


    void Update()
    {
        // checks if the player has entered the second area
        if (BridgeEventScritp.bridgeEventTriggered == false && called)
        {
            StartCoroutine("CopCooldown");
            called = false;
        }
    }

    IEnumerator CopCooldown()
    {
        // Spawns cops 1 second after the eventTrigger is called
        yield return new WaitForSeconds(1);
        transform.GetChild(2).GetComponent<CopSpawner>().spawnCop(timeBetweenSpawns, maxCops, copSpawnPoint, copLifeTime, copPrefab);
    }
}
