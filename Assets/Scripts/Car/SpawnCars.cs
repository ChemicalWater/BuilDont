// Code by Eele Roet
using Pathfinding;
using System.Collections;
using UnityEngine;

/// <summary>
/// Simple car prefab spawner.
/// Instantiates a stream of random cars using the serialized information.
/// </summary>

public class SpawnCars : MonoBehaviour
{
    //settings for spawning the cars
    [SerializeField] private int maxCars = 15;
    [SerializeField] private float carSpawnInterval = 3;
    [SerializeField] GameObject[] carsToSpawn;   //a list of possible cars to spawn
    [SerializeField] GameObject TargetsForAStar; //a gameobject that contains the path the cars drive

    //counter that counts how many cars have spawned
    private int carCounter;

    // Start is called before the first frame update
    void Start()
    {
        carCounter = 0;

        //cars start spawning when the game is loaded
        StartCoroutine(SpawnCarsInIntervals());
    }

    private IEnumerator SpawnCarsInIntervals()
    {
        //a while loop runs until all cars are spawned
        while (carCounter < maxCars)
        {
            //each loop the method pauses the length of the interval before spawning a new car
            yield return new WaitForSeconds(carSpawnInterval);
            carCounter++;

            //a new random car is initiated on the location of the spawner,
            //the pathfinding targets are then passed to the Patrol component of the car.
            GameObject newCar = Instantiate(carsToSpawn[Random.Range(0, carsToSpawn.Length - 1)], transform);
                       newCar.GetComponent<Patrol>().TargetsAreChildren = TargetsForAStar;
        }
    }
    
}
