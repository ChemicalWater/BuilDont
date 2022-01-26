using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerplantScript : MonoBehaviour
{
    private Vector3 center;
    [SerializeField]
    float radius; // radius for the power effect, within this radius all the building their lights are on

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip runningSound;
    [SerializeField] AudioClip shutdownSound;
    [SerializeField] DisableCameraZones disableZones;

    private GameObject brokenparts;
    private bool didIbreak;

    private bool finClearing;

    // Start is called before the first frame update
    void Start()
    {
        //sound of powerstation working
        source.clip = runningSound;
        source.Play();

        center = this.transform.position;
        // Get all the lights from buildings in the radius
        StartCoroutine(getLights());
        brokenparts = this.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // If broken gameobject is on and we are not broken yet
        if (brokenparts.activeSelf && !didIbreak)
        {
            // Get all the lights again so we update it without all the already broken buildings
            StartCoroutine(getLights());
            // Focus the camera on the draw bridge
            RTSCamera.UpdateFocus(new Vector3(154f, -3f, 201f), 0.8f);

            //shutdown sound
            source.loop = false;
            source.clip = shutdownSound;
            source.Play();

            //turn off the camera zones of the area
            disableZones.disableFocusZones();

            didIbreak = true;
        }
    }

    IEnumerator getLights()
    {
        // Wait for a sec
        yield return new WaitForSeconds(1);

        // Put all overlapping colliders inside our light array
        Collider[] lights = Physics.OverlapSphere(center, radius);

        for (int i = 0; i < lights.Length; i++)
        {
            // if the tag of the light is not what we want, remove them
            if (lights[i].tag != "building" && lights[i].tag != "drawbridge")
            {
                lights[i] = null;
            }
        }

        foreach (var light in lights)
        {
            // For all the lights we still have, check if their tag is building or drawbridge
            if (light != null)
            {
                // Is the tag a building? turn off their lights
                if (light.tag == "building")
                light.GetComponent<BuildingScript>().LightSwitch();

                // is the tag the drawbridge and I am actually broken, call for the bridge to be lowered
                if (light.tag == "drawbridge" && didIbreak)
                    light.GetComponent<DrawBridgeScript>().lowerBridge();
            }
        }
    }
}
