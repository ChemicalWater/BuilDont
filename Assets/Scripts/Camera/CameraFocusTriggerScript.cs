//code by Eele Roet
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the camera focus when the player enters the trigger collider this script is attached to.
/// Uses the transform of the FocusPointCube and a serialized float to set the camera focus.
/// </summary>
public class CameraFocusTriggerScript : MonoBehaviour
{
    [SerializeField] Transform FocusPoint;
    [SerializeField] float amountOfZoom;
    [SerializeField] GameObject FocusPointCube;

    // Start is called before the first frame update
    void Start()
    {
        //turns off the cube so its not floating in the world
        FocusPointCube.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        //when the player walks into the trigger collider UpdateFocus is called.
        if(other.tag == "Player")
        {
           RTSCamera.UpdateFocus(FocusPoint.position, amountOfZoom);
        }
    }
}
