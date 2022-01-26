//Code by Eele Roet
using UnityEngine;

/// <summary>
/// <para> Handles the collisions between the hammer and other colliders</para>
/// <para> On collision this script calls to destroy buildings and cars, </para>
/// <para>also checks the type of hammer used</para> 
/// </summary>
public class HandleColliderToggle : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        BuildingScript buildScript = other.gameObject.GetComponent<BuildingScript>();
        //if the other collider is from a building, destroy the building
        if (buildScript != null)
        {
            buildScript.CollapseBuilding();//break the building
            //if the golden hammer was used 
            if(transform.parent.gameObject.name == "gold")
            {
                buildScript.TurnRubbleToGold();//turn the rubble into a gold material
            }
        }
        //if the other collider is not a trigger collider
        else if (!other.isTrigger)
        {
            CarLaunchScript launchScript = other.gameObject.GetComponent<CarLaunchScript>();
            //if the other collider is from a car, launch or break it
            if (launchScript != null)
            {
                //if the rubber hammer was used, launch the car in the direction the player is facing
                if (transform.parent.gameObject.name == "rubber")
                {
                    launchScript.LaunchCar(transform.parent.parent.parent.forward);//convoluted way to get the forward direction of the player
                }
                //if another hammer was used, only break the car
                else
                {
                    launchScript.BreakCar();
                }
            }
        }
    }
}
