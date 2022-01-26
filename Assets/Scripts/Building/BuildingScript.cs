using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    private GameManager manager;
    private GameObject normal;
    private GameObject broken;
    private GameObject player;
    private int BoxColliderDisableDelay = 7;

    private GameObject lights;

    BoxCollider[] BuildingColliders;

    private bool cleanup;
    private bool TenSecWait;

    private float playerdist;

    public bool bankLockdownEnd;
    public bool banklockdownStart;

    [SerializeField] private Material goldRubble;
    [SerializeField] GameObject itemPickUp;
    [SerializeField] private bool IfHardwareStoreDropLadder;
    [SerializeField] private float amountOfRubble = 5.0f;

    private Rigidbody[] bodys;
    // Start is called before the first frame update
    void Start()
    {
        // Get the gamemanager script from the hud gameobject
        manager = GameObject.FindGameObjectWithTag("HUD").GetComponent<GameManager>();
        // Get the way the building looks before broken from it's first child
        normal = this.gameObject.transform.GetChild(0).gameObject;
        // Get the way the building looks broken from it's second child
        broken = this.gameObject.transform.GetChild(1).gameObject;
        // Get all the rigidbodies from the broken pieces
        bodys = broken.GetComponentsInChildren<Rigidbody>();
        // Make sure the building does not break before spawning or spawn broken
        broken.SetActive(false);
        // find the player
        player = GameObject.Find("Player");

        // Check if the normal has any children and if so get the lights from the first child
        if(normal.transform.childCount != 0)
        lights = normal.gameObject.transform.GetChild(0).gameObject;
        // Get all the buildings colliders
        BuildingColliders = gameObject.GetComponents<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

        if (cleanup && TenSecWait)
        { // Get the players distance from this gameobject
            playerdist = Vector3.Distance(player.transform.position, this.transform.position);
            // check if the player distance is far away from the building
            if (playerdist > 25 && this.tag == "building" || playerdist > 25 && this.tag == "powerplant" || playerdist < 25 && this.tag == "hardwarestore" ||
                playerdist < 25 && this.tag == "fireworkStore" || playerdist < 25 && this.tag == "dambuilding" || playerdist > 30 && this.tag == "bank")
            {
                // Turn every broken piece to kinematic, so it no longer has physics
                for (int i = 0; i < bodys.Length; i++)
                {
                    if (bodys[i] != null)
                    bodys[i].isKinematic = true;
                }
                cleanup = false;

                // Start checking if the player is close to the building
                StartCoroutine(CheckPlayerBuilding());
            }
        }
    }

    public void TurnRubbleToGold()
    {
        // Go through all broken pieces rigidbodies, change their material to our goldmaterial and check it off on goals
        foreach(Rigidbody body in bodys)
        {
            body.gameObject.GetComponent<MeshRenderer>().material = goldRubble;
            manager.AddGoalProgressByType(GoalType.BuildingGold);
        }
    }

    public void LightSwitch()
    {
        // if the lights are found
        if (lights != null)
        {
            // Turn the lights off if the lights were already on
            if (lights.activeSelf)
                lights.SetActive(false);
            else
                // turn the lights on if the lights weren't already on
                lights.SetActive(true);
        }
    }

    public void CollapseBuilding()
    {
        // So long this is not the bank, or when it is the bank and the lockdown has ended
        if (this.tag != "bank" || this.tag == "bank" && bankLockdownEnd)
        {
            // The building is intact
            if (normal.activeSelf)
            {
                // Start our cleanup program, disable all the box colliders and go check if this building drops an item
                StartCoroutine(SafeOurFrames());
                StartCoroutine(DisableBoxCollider());
                PickUpItem();
                // depending on what tag this building has, we check off the goal if it's there
                switch (tag)
                {
                    case "building":
                    case "fireworkstore":
                    case "hardwarestore":
                        manager.AddGoalProgressByType(GoalType.Building);
                        break;
                    case "policestation":
                            manager.AddGoalProgressByType(GoalType.PoliceBuilding);
                        // Destroy the spawnpoint of the policestation to make sure cops stop spawning
                        Destroy(transform.GetChild(2).gameObject);
                        break;
                    case "bank":
                        manager.AddGoalProgressByType(GoalType.Bank);
                        break;
                    case "powerplant":
                        manager.AddGoalProgressByType(GoalType.Powerstation);
                        break;
                    case "dambuilding":
                        manager.AddGoalProgressByType(GoalType.Dam);
                        // Look for the dam and call the destruction of the dam itself
                        transform.parent.Find("Damn").GetComponent<DamDesScript>().DestroyDam();
                        // update the camera so it follows the waterflow
                        RTSCamera.UpdateFocus(new Vector3(234f, -3f, 99f), 0f);
                        break;
                }
                // Turn off the intact building and show the broken pieces
                normal.SetActive(false);
                broken.SetActive(true);
            }
        }
        // if this building is a bank and the lockdown has not started yet, start the lockdown
        if (this.tag == "bank" && !banklockdownStart && !bankLockdownEnd)
        {
            gameObject.GetComponent<bankScript>().StartCoroutine("lockDown");
            // Make sure we get rid of most of the rubble by setting a value here
            amountOfRubble = 3.0f;
        }
    }

    IEnumerator CheckPlayerBuilding()
    {
        // As long as the coroutine is active, cleanup hasn't started, building is destroyed and we are waiting for 10 seconds
        while (true) {
            if (!cleanup && broken.activeSelf && TenSecWait)
            {
                // Get the distance from the player to the building
                playerdist = Vector3.Distance(player.transform.position, this.transform.position);
                if (playerdist < 12 && this.tag == "building" || playerdist > 25 && this.tag == "powerplant" || playerdist < 20 && this.tag == "hardwarestore" ||
                    playerdist < 18 && this.tag == "fireworkStore" || playerdist < 18 && this.tag == "dambuilding" || playerdist > 25 && this.tag == "bank")
                {
                    // if the player is close to the building, turn kinematic back off so the player can move rubble around
                    for (int i = 0; i < bodys.Length; i++)
                    {
                        if (bodys[i] != null)
                            bodys[i].isKinematic = false;
                    }
                    // Start cleanup
                    cleanup = true;
                    // stop the coroutine
                    StopCoroutine(CheckPlayerBuilding());
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator SafeOurFrames()
    {
        // Wait for a few seconds, turn on cleanup and tensecwait, wait for few more seconds
        yield return new WaitForSeconds(3);
        cleanup = true;
        TenSecWait = true;
        yield return new WaitForSeconds(5);

        // start removing broken pieces to decrease frame drops
        for (int i = bodys.Length-1; i > (bodys.Length/amountOfRubble) ; i--)
        {
            if (bodys[i] != null)
            {
              Destroy(bodys[i].gameObject);
              bodys[i] = null;
            }
            yield return null;
        }
    }

    IEnumerator DisableBoxCollider()
    {
        int counter = 0;
        // keep adding numbers to the counter until we reach the same number
        while(counter < BoxColliderDisableDelay)
        {
            yield return null;
            counter++;
        }
        // turn off every boxcollider in buildingcolliders so the player can walk through the old building
            foreach (BoxCollider bc in BuildingColliders)
                bc.enabled = false;
    }

    void PickUpItem()
    {
        // check if this building is not any of these tags
        if (this.tag != "building" && this.tag != "powerplant" && this.tag != "dambuilding" && this.tag != "policestation")
        {
            // if this building is a hardwarestore, create itempickup gameobject and pass through the ladder item
            if(IfHardwareStoreDropLadder)
            {
                GameObject ladder = Instantiate(itemPickUp, new Vector3(this.transform.position.x, (this.transform.position.y + 1.5f), this.transform.position.z), this.transform.rotation);
                ladder.GetComponent<PickUp>().GetItem("ladder");
            }
            else
            {  // if this building is a hardwarestore but we don't want to drop a ladder, create itempickup gameobject and pass through our tag
                GameObject Item = Instantiate(itemPickUp, new Vector3(this.transform.position.x, (this.transform.position.y + 1.5f), this.transform.position.z), this.transform.rotation);
                Item.GetComponent<PickUp>().GetItem(this.tag);
            }
        }
    }
}
