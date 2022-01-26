//Code by Eele Roet
using UnityEngine;

/// <summary>
/// <para>Script that disables camera focus zones.</para>
/// <para>Camera zones from old areas are turned off to make moving to new areas easier</para>
/// </summary>
public class DisableCameraZones : MonoBehaviour
{
    [SerializeField] private CameraFocusTriggerScript[] focusZones;//array of camera zones that are turned off after an event

    //method that turns off the camera zones
    public void disableFocusZones()
    {
        //loops through all zones
        foreach (CameraFocusTriggerScript focus in focusZones)
        {
            if (focus != null)
            {
                Destroy(focus.gameObject);//destroys the zone
            }
        }
    }
}

