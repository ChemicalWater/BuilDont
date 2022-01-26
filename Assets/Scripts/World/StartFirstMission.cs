using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFirstMission : MonoBehaviour
{
    private bool started = false;
    [SerializeField] private GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !started)
        {
            manager.StartFirstMission();
            started = true;
        }
    }
}
