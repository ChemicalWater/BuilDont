using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopSpawner : MonoBehaviour
{
    float time;
    int currentCops;
    int maximum;
    int lifeTimer;
    GameObject cops;
    Transform spawn;

    public static ArrayList activeCops = new ArrayList();

    /// <summary>
    /// Method to spawn in certain entities with a lifetime and maximum amount.
    /// </summary>
    /// <param name="timeBetween"> Time between 2 entities spawning.</param>
    /// <param name="maxCops"> Maximum amount of entities.</param>
    /// <param name="position"> Transform of the position of where the entities are gonna spawn</param>
    /// <param name="lifeTime"> Lifetime of the entities (Keep 0 for infinite lifetime)</param>
    /// <param name="cop"> prefab of the entity that is going to spawn</param>
    public void spawnCop(float timeBetween, int maxCops, Transform position, int lifeTime, GameObject cop)
    {
        time = timeBetween;
        maximum = maxCops;
        cops = cop;
        spawn = position;
        lifeTimer = lifeTime;
        StartCoroutine("copTimer");
    }

    IEnumerator copTimer()
    {
        while (currentCops < maximum)
        {
            yield return new WaitForSeconds(time);
            GameObject cop = Instantiate(cops, spawn, false);
            cop.GetComponent<CopHitScript>().copBuilding = this.transform;
            activeCops.Add(cop.GetComponent<CopHitScript>());
            currentCops++;
            if (lifeTimer >= 1)
            {
                Destroy(cop, lifeTimer);
                currentCops--;
            }
        }
    }
}
