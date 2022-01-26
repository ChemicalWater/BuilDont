//Code by Eele Roet
using Pathfinding;
using System.Collections;
using UnityEngine;

/// <summary>
/// <para> Handles all the car behavior besides the pathfinding.</para>
/// <para>Plays the car sounds, handles hitting the player while driving,</para> 
/// <para>handles being destroyed by the player, handles getting launched by the rubber hammer and then destoying building.</para>
/// </summary>
public class CarLaunchScript : MonoBehaviour
{
    private GameManager manager;

    [SerializeField] private bool driving;//when true the player gets raggdolled on contact with the car, is false when destroyed or the car is stationary
    [SerializeField] private Rigidbody rb;//car rigidbody
    [SerializeField] private float forceAmount;//force that the car gets launched with
    [SerializeField] private BoxCollider triggerCollider;//collider that is enabled when the car is launched, destroys buildings when they enter
    [SerializeField] private GameObject normalModel;//non-broken car model
    [SerializeField] private GameObject brokenModel;//broken car model
    [SerializeField] private AudioSource carSoundSource;//audio source that plays driving and smash sounds
    [SerializeField] private AudioClip[] smashSounds;//sounds for when the car gets destroyed
    [SerializeField] private AudioClip[] vroomAndBeepSounds;//driving and horn sounds
    [SerializeField] private AudioSource collisionSoundSource;//audios source that plays player-car collision sounds
    [SerializeField] private AudioClip playerCarCollisionSound;//player getting hit by a car sound

    private float triggerStaysOnForSec = 3f;//after this amount of seconds the trigger collider can't destroy buildings anymore
    private float extraYLift = 0.2f;//extra force thats added to the y axis when the car gets launched
    private int buildingsDestroyedWithThisCar = 0;//counter for the mission "destroy 5 buildings with a single car"

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("HUD").GetComponent<GameManager>();
        //if the car drives around the map
        if (driving)
        {
            //the player can get hit by the trigger collider
            triggerCollider.enabled = true;
            //car sounds randomly play
            StartCoroutine(PlayVroomAndBeepSounds());
        }
    }

    private IEnumerator PlayVroomAndBeepSounds()
    {
        //keeps running as long as the car is not destroyed by the player
        while(true)
        {
            float r = Random.Range(0f, 1f);
            //90% of the loops the car will play the engine sound for 5 seconds
            if(r > 0.1)
            {
                //play the vroom sound for 5 seconds
                carSoundSource.clip = vroomAndBeepSounds[0];//engine sound gets set
                carSoundSource.Play();
                yield return new WaitForSeconds(5);
            }
            //10% of the loops the car will honk
            else
            {
                //play the beep sound for the beep length
                carSoundSource.clip = vroomAndBeepSounds[1];//horn sound gets set
                carSoundSource.Play();
                yield return new WaitForSeconds(vroomAndBeepSounds[1].length);
            }
        }
    }

   
    //this method handles the car flying away when hit with the rubber hammer or a bomb.
    //it takes in a direction to launch the car in.
    public void LaunchCar(Vector3 launchDirection)
    {
        launchDirection.Normalize();//normalize the direction
        launchDirection.y += extraYLift;//add the extra y lift
        rb.AddForce(launchDirection * forceAmount);//add the force to the rigidbody to let unity handle the flying away

        StopCoroutine(ToggleTriggerCollider());//stop the coroutine if it was running
        StartCoroutine(ToggleTriggerCollider());//start the coroutine to keep the trigger collider on for the specified amount of time
        
        BreakCar();//turn off the driving behaviors.
        triggerCollider.enabled = true;

        manager.AddGoalProgressByType(GoalType.CarRubber);//add progress to the goal, if the current goal is to launch cars

    }

    public void BreakCar()
    {
        //if the car is not broken
        if (normalModel.activeSelf)
        {
            //switch to the broken model
            normalModel.SetActive(false);
            brokenModel.SetActive(true);

            //if the car is driving around
            if (driving)
            {
                //don't do that anymore
                driving = false;
                triggerCollider.enabled = false;//player can't get hit by the car anymore
                GetComponent<Patrol>().enabled = false;//turns off the pathfinding
                GetComponent<AIPath>().enabled = false;//turns off the pathfinding
                StopCoroutine(PlayVroomAndBeepSounds());//stops the sounds
            }
        }
        if (gameObject.tag == "CopCar")
        {
            manager.AddGoalProgressByType(GoalType.PoliceCar);//add progress to the goal, if the current goal is to destroy copCars
        }
        PlaySmashSound();//play the sound of a car getting destroyed
    }

    private void PlaySmashSound()
    {
        carSoundSource.clip = smashSounds[Random.Range(0, smashSounds.Length)];//sets clip to random sound out of the array
        carSoundSource.Play();
    }

    private IEnumerator ToggleTriggerCollider()
    {
        triggerCollider.enabled = true;
        yield return new WaitForSeconds(triggerStaysOnForSec);//wait for a while
        triggerCollider.enabled = false;//the car can't destroy buildings anymore
    }

    void OnTriggerEnter(Collider other)
    {
        //if the car is broken, check collisions with buildings
        if (brokenModel.activeSelf)
        {
            BuildingScript buildScript = other.gameObject.GetComponent<BuildingScript>();
            //if the thing the car collides with is a buiding, destroy it
            if (buildScript != null)
            {
                buildScript.CollapseBuilding();//destroy the building
                triggerCollider.enabled = false;//can't destroy another building till the car is launched again
                manager.AddGoalProgressByType(GoalType.CarSmashBuild1Time);//add progress to the goal, if the current goal is to smash buildings with cars
                if (++buildingsDestroyedWithThisCar >= 5)
                {
                    manager.AddGoalProgressByType(GoalType.CarSmashBuild5Times);//add progress to the goal, if the current goal is to smash 5 buildings with this car
                }
            }
        }
        //if the car is not broken, check collisions with the player
        else if(other.tag == "Player" && driving)
        {
            PlayerMovementScript.Ragdoll(other.transform.position - transform.position);//toss the player about
            collisionSoundSource.clip = playerCarCollisionSound;//set the collision sound
            collisionSoundSource.Play();
        }
    }
}
