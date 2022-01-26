using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInputManager : InputManager
{
    // EVENTS
    public static event MoveInPutHandler moveOnInput;
    public static event RotateInPutHandler rotateOnInput;
    public static event ZoomInputHandler zoomOnInput;

    private void Update()
    {
        //Movement
        if (Input.GetKey(KeyCode.W))
        {
            moveOnInput?.Invoke(Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveOnInput?.Invoke(-Vector3.forward);
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveOnInput?.Invoke(Vector3.right);
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveOnInput?.Invoke(-Vector3.right);
        }
        //Rotation
        if (Input.GetKey(KeyCode.E))
        {
            rotateOnInput?.Invoke(-1f);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rotateOnInput?.Invoke(1f);
        }
        //Zoom
        
        if (Input.GetKey(KeyCode.Z))
        {
            zoomOnInput?.Invoke(1f);
        }
        if (Input.GetKey(KeyCode.X))
        {
            zoomOnInput?.Invoke(-1f);
        }
        
    }
}
