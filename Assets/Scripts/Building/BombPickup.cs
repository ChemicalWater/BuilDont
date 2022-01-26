using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InventoryScript.AddUsable(ItemType.bomb);
            Destroy(this.gameObject);
        }
    }
}
