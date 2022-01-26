using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerBombScript : MonoBehaviour
{
    private GameManager manager;
    Vector3 center;
    private float radius;
    [SerializeField]
    GameObject explosion;

    void Start()
    {
        
        manager = GameObject.FindGameObjectWithTag("HUD").GetComponent<GameManager>();
        center = this.transform.position;
        radius = 5;
        manager.AddGoalProgressByType(GoalType.Bomb);
        StartCoroutine("Explode");
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(3); 
        Destroy(gameObject);
        explosion = Instantiate(explosion, transform.position, transform.rotation);
        explosion.transform.GetChild(0).transform.localScale = new Vector3(3, 3, 3);
        OnExplode();
        Destroy(explosion, 2);
    }

    void OnExplode()
    {
        
        Collider[] explode = Physics.OverlapSphere(center, radius);

        foreach (var Object in explode)
        {
            if (Object.tag == "building" || Object.tag == "powerplant" || Object.tag == "hardwarestore" || Object.tag == "bank" || Object.tag == "policestation")
            {
                Object.GetComponent<BuildingScript>().CollapseBuilding();
            }
            else if (Object.tag == "fence")
            {
                Object.GetComponent<FenceScript>().DestroyFence();
            }
            else if(Object.tag == "car" || Object.tag == "CopCar")
            {
                Object.GetComponent<CarLaunchScript>().LaunchCar(Object.transform.position - transform.position);
            }

        }
    }
}
