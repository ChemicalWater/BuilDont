// Code by Eele Roet
using UnityEngine;

/// <summary>
/// Manages the player camera.
/// Keeps track of Zoom and rotation, automatically focuses on specified point in the world.
/// </summary>
public class RTSCamera : MonoBehaviour
{
    private static Vector2 zoomRange = new Vector2(-2, 15); //minimum and maximum zooms
    public float CurrentZoom = 0;
    private static float TargetZoom = 0;//goal zoom that is adjusted by mousescrolling
    public float ZoomSpeed = 1;
    public float ZoomRotation = 1;
    public Vector2 zoomAngleRange = new Vector2(20, 70);//the lower and upper bounds of zoom on the x axis

    public float rotateSpeed = 180f;//the speed with which the camera turns left and right
    private static Vector3 FocusPoint = Vector3.zero;//the point the camera turns to

    private Vector3 initialPosition;//used as point of reference when adjusting position
    private Vector3 initialRotation;//used as point of reference when adjusting zoom

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraAnchor;


    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.eulerAngles;
    }


    void Update()
    {
        // Track playerposition
        cameraAnchor.position = playerTransform.position;

        // zoom in/out
        CurrentZoom += Mathf.Clamp(TargetZoom-CurrentZoom,-ZoomSpeed, ZoomSpeed) * Time.deltaTime * 1 * ZoomSpeed;
        CurrentZoom = Mathf.Clamp(CurrentZoom, zoomRange.x, zoomRange.y);

        transform.localPosition = new Vector3(transform.localPosition.x, initialPosition.y + CurrentZoom, initialPosition.z - CurrentZoom * 0.5f);

        //rotate up/down on x axis
        float x = transform.eulerAngles.x - (transform.eulerAngles.x - (initialRotation.x + CurrentZoom * ZoomRotation)) * 0.1f;
        x = Mathf.Clamp(x, zoomAngleRange.x, zoomAngleRange.y);
        transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);

        //rotate left/right on y axis 
        //get clamped difference in rotation between target and current
        Vector2 currentRotation = new Vector2(transform.forward.x, transform.forward.z);
        currentRotation.Normalize();
        Vector2 fromPlayerToTarget = new Vector2(FocusPoint.x, FocusPoint.z) - new Vector2(playerTransform.position.x, playerTransform.position.z);
        float amountToRotate = Vector2.SignedAngle(currentRotation, fromPlayerToTarget);

        //Rotate around on y axis with clamped difference
        cameraAnchor.Rotate(Vector3.up, Mathf.Clamp(-amountToRotate, -rotateSpeed, rotateSpeed) * Time.deltaTime);
    }

    //method used to set a new focus point and zoom 
    public static void UpdateFocus(Vector3 pointToLookAt, float newZoom)
    {
        FocusPoint = pointToLookAt;
        TargetZoom = Mathf.Lerp(zoomRange.x, zoomRange.y, newZoom);
    }
}

