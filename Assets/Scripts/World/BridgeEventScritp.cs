using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeEventScritp : MonoBehaviour
{
    public static bool bridgeEventTriggered = true;
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bridgeEventTriggered)
        {
            if (other.tag.Contains("Player"))
            {
                Debug.Log("gay");
                DayNightCycle.TurnDay(25);
                bridgeEventTriggered = false;
            }
        }
    }
}
