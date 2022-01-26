using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputManager : MonoBehaviour
{
    public delegate void MoveInPutHandler(Vector3 moveVector);
    public delegate void RotateInPutHandler(float rotateAmount);
    public delegate void ZoomInputHandler(float zoomAmount);
}
