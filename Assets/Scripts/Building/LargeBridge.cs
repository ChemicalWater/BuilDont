using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LargeBridge : MonoBehaviour
{
    private GameManager manager;

    [SerializeField] // bridge first part
    GameObject br_p1;

    [SerializeField] GameObject payCanvas; // GameObject with TextMeshPro item on it

    [SerializeField] GameObject br_pillar_1; // first breakable pillar
    [SerializeField] GameObject br_pillar_2; // second breakable pillar
    [SerializeField] GameObject br_pillar_3; // third breakable pillar
    [SerializeField] private AudioSource bridgebreaking;

    [SerializeField] // Text shown after player steps towards NPC
    private string getMoneyText = "I'm not letting you on, atleast.. not for free.";

    [SerializeField] // Text shown after player steps away from NPC 10 times
    private string easterEggText = "You've had your chance, now f*ck off.";

    [SerializeField] // bridge second part
    GameObject br_p2;

    [SerializeField] // bridge third part
    GameObject br_p3;

    private int timesWalked; // How many times has the player stepped away from the NPC

    private BoxCollider[] payBox; // All the colliders near the NPC

    public bool BridgeIsCollapsed; // Check if the bridge is collapsed or if it's still intact

    private Animation br_p1_anim; // animation for breaking part 1 of the bridge
    private Animation br_p2_anim; // animation for breaking part 2 of the bridge
    private Animation br_p3_anim; // animation for breaking part 3 of the bridge

    private TextMeshPro payBoxText; // TextMeshPro Item

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("HUD").GetComponent<GameManager>(); // Get the gamemanager script from the hud gameobject
        br_p1_anim = br_p1.GetComponent<Animation>(); // Get the animation for part 1
        br_p2_anim = br_p2.GetComponent<Animation>(); // get the animation for part 2
        br_p3_anim = br_p3.GetComponent<Animation>(); // get the animation for part 3

        payBoxText = payCanvas.GetComponent<TextMeshPro>(); // Get TextMeshPro from paycanvas add it to the TextMeshPro item
        payBoxText.text = ""; // Since the player is not near, put the text on empty
        payBox = GetComponents<BoxCollider>(); // get the colliders and add them to the collider array.
    }

    void Update()
    {
        // If the bridge is collapsed and all the pillars are no longer intact
        if (!BridgeIsCollapsed && !br_pillar_1.activeSelf && !br_pillar_2.activeSelf && !br_pillar_3.activeSelf)
        {
            CollapseBridge();
        }

    }

    void CollapseBridge()
    {
        // play all the bridge animations + sound, set bridge to collapsed and call it a win
        BridgeIsCollapsed = true;
        bridgebreaking.Play();
        br_p1_anim.Play();
        br_p2_anim.Play();
        br_p3_anim.Play();
        manager.WinGame();
    }

    void OnTriggerStay(Collider other)
    {
        // check if the player collides with our collider and if he has not walked away more than 9 times already
        if (other.tag == "Player" && timesWalked <= 9)
        {
            // If the player has the money item in his inventory and he currently has it in his hands
            if (InventoryScript.GetInventoryItemByType(ItemType.moneybag) != null && InventoryScript.currentUsable.GetItemType() == ItemType.moneybag)
            {
                // Check the goal off from the list and make sure the item gets used
                manager.AddGoalProgressByType(GoalType.PayNPC);
                InventoryScript.UseUsable();
                // Disable all the colliders in our collider array so the player can walk through towards the pillars
                foreach (BoxCollider bc in payBox)
                    bc.enabled = false;

            } else if (timesWalked <= 9)
            {
                // Make sure the player has his goal checked off and change the displaying text to our new text
                manager.AddGoalProgressByType(GoalType.FindNPC);
                payBoxText.text = getMoneyText;
            }
        } else if (other.tag == "Player")
        {
            // Change the text towards our eastereggtext and call the lose function
            payBoxText.text = easterEggText;
            manager.LoseGame();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // if the player walks away from the NPC, count one and change the text back to empty so it won't be in the players way
        timesWalked++;
        payBoxText.text = "";
    }
}
