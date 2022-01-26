using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light moonLight;
    [SerializeField] private Light sunLight;
    static float targetRotation = 180;
    private float rotationSpeed = 24f;
    private float currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the start rotation so that the game always starts at night.
        currentRotation = 180;
    }

    /// <summary>
    /// Sends a new rotation value to the Update
    /// </summary>
    /// <param name="newRotation">New rotation of the sun</param>
    public static void TurnDay(float newRotation)
    {
        targetRotation = newRotation;
    }
    // Update is called once per frame
    void Update()
    {
        float rotationDiff = targetRotation - currentRotation;

        currentRotation += Mathf.Clamp(rotationDiff, -rotationSpeed, rotationSpeed) * Time.deltaTime;
        gameObject.transform.eulerAngles = new Vector3(currentRotation, 0, 0);

        if (currentRotation%360 >= 90 && currentRotation%360 <= 270)
        {
            if (sunLight.intensity >= 0)
            {
                sunLight.intensity -= 0.005f;
            } 
        }
        else 
        {
            if (sunLight.intensity <= 0.8f)
            {
                sunLight.intensity += 0.005f;
            }
        }       
    }
}
