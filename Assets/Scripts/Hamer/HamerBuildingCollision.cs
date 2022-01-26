//Code by Eele Roet
using System.Collections;
using UnityEngine;

/// <summary>
/// <para> Handles enabling and disabling the collider in the hammer</para>
/// <para> The actual collision handling is done in the HandleColliderToggle script on the handle of the active hammer, sorry for the confusing names</para> 
/// </summary>
public class HamerBuildingCollision : MonoBehaviour
{
    BoxCollider hamerCollider;//collider of the active hammer

    //public method that gets called in the hammer swing animation.
    public void EnableHammerCollider()
    {
        StartCoroutine("ToggleCollider");//turn the collider on then off after 3 frames
    }

    private IEnumerator ToggleCollider()
    {
        int counter = 0;//frame counter
        //looks through the hammers for the active hammer
        foreach (Transform child in GetComponentInChildren<Transform>())
        {
            //when the active hammer is found
            if (child.gameObject.activeSelf)
            {
                hamerCollider = child.GetChild(0).GetComponent<BoxCollider>();//hammer collider is set to the collider of the active hammer
            }
        }
        hamerCollider.enabled = true;//enable the collider
        //wait three frames
        while (counter < 3)
        {
            yield return null;
            counter++;
        }
        hamerCollider.enabled = false;//disable the collider
    }
}
