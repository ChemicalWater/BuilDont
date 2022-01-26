using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private GameManager manager;

    [SerializeField]
    GameObject bomb;
    [SerializeField]
    GameObject ladder;
    [SerializeField]
    GameObject stoneHammer;
    [SerializeField]
    GameObject elekHammer;
    [SerializeField]
    GameObject goldHammer;
    [SerializeField]
    GameObject rubberHammer;
    [SerializeField]
    GameObject moneybag;
    [SerializeField]
    AudioSource source;
    [SerializeField]
    AudioClip pickupSound;

    [Range(0, 30)]
    [SerializeField] float spinningSpeed = 15;

    private ItemType givenItem;

    private void Start()
    {
        // Get the gamemanager script from the hud gameobject
        manager = GameObject.FindGameObjectWithTag("HUD").GetComponent<GameManager>(); 
    }

    void Update()
    {
        // Rotate the gameobject sideways * frames * speed we decide
        transform.Rotate(Vector3.up * Time.deltaTime * spinningSpeed);
    }

    public void GetItem(string itemname)
    {
        // if the given itemname is the same as this string
        if (itemname == "hardwarestore")
        {
            // get the last hammer the player has picked up from inventory script
            switch (InventoryScript.GetLastHammerPickedUp())
            {
               case ItemType.stoneHammer: 
                goldHammer.SetActive(true);
                    givenItem = ItemType.goldHammer;
                break;
                case ItemType.goldHammer:
                    rubberHammer.SetActive(true);
                    givenItem = ItemType.rubberHammer;
                    break;
                case ItemType.rubberHammer:
                    elekHammer.SetActive(true);
                    givenItem = ItemType.elekHammer;
                    break;
                case ItemType.elekHammer:
                    Debug.Log("You've got all the hammers!");
                    break;
            }
        }
        else if (itemname == "fireworkstore")
        {
            bomb.SetActive(true);
            givenItem = ItemType.bomb;
        }
        else if(itemname == "ladder")
        {
            ladder.SetActive(true);
            givenItem = ItemType.ladder;
        } else if (itemname == "bank")
        {
            moneybag.SetActive(true);
            givenItem = ItemType.moneybag;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // if the player walks into the item, give the player the item that has been dropped
            switch (givenItem)
            {
                case ItemType.bomb:
                    InventoryScript.AddUsable(givenItem);
                    break;
                case ItemType.moneybag:
                    InventoryScript.AddUsable(givenItem);
                    break;
                case ItemType.ladder:
                    InventoryScript.AddUsable(givenItem);
                    manager.AddGoalProgressByType(GoalType.Ladder);
                    break;
                case ItemType.stoneHammer:
                case ItemType.goldHammer:
                case ItemType.rubberHammer:
                case ItemType.elekHammer:
                    InventoryScript.AddHammer(givenItem);
                    break;
                default:
                    Debug.Log("givenItem is not an existing ItemType");
                    break;
            }
            
            foreach (Transform child in transform.GetComponentInChildren<Transform>())
            {
                // turn off every child so the gameobject is no longer visible
                if(child != this.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }

            StartCoroutine(PlaySoundThenDestroy());
        }
    }

    private IEnumerator PlaySoundThenDestroy()
    {
        // play the sound, wait before the sound is done and then destroy the gameobject
        source.clip = pickupSound;
        source.Play();
        yield return new WaitForSeconds(pickupSound.length);
        Destroy(gameObject);
    }
}
