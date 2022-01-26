using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBridgeScript : MonoBehaviour
{
    [SerializeField]
    GameObject bridge;

    private Animation brAnim;
    private BoxCollider brColl;

    // Start is called before the first frame update
    void Start()
    {
        // Get the bridge animation and the bridges collider
        brAnim = bridge.GetComponent<Animation>();
        brColl = GetComponent<BoxCollider>();
    }

    public void lowerBridge()
    {
        // play the animation and turn off the collider
        brAnim.Play();
        brColl.enabled = false;
    }
}
