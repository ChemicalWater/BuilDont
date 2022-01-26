using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopHitScript : MonoBehaviour
{
    [Tooltip("The Higher the value, the higher the chance of a male cop spawning")]
    [Range(0, 1)] public float maleToFemaleRatio;

    [SerializeField] private int copWalkingAwayTimer;
    [SerializeField] public Pathfinding.AIDestinationSetter ai;
    [SerializeField] private AudioSource hitSoundSource;
    [SerializeField] private AudioClip hitSound;

    public Transform copBuilding;
    private Transform player;

    /// <summary>
    /// Here the bulk of the Cop is initialized.
    /// </summary>
    private void Start()
    {
        // Finds the transform of the player and sets it as a target for the pathfinding AI
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ai.target = player;

        // Creates a randomizer that selects between female and Male cop skin (consistency can be edited in the editor)
        if (Random.value < maleToFemaleRatio)
        {
            transform.GetChild(0).Find("Character_Male_Police").gameObject.SetActive(true);
            transform.GetChild(0).Find("Character_Female_Police").gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).Find("Character_Male_Police").gameObject.SetActive(false);
            transform.GetChild(0).Find("Character_Female_Police").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Collider check for playerCop collision
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //Checks if the collider is touching a player.
        if (collision.gameObject.tag == "Player")
        {
            // Adds a ragdoll to the player so that the player flies away
            PlayerMovementScript.Ragdoll(collision.gameObject.transform.position - (this.transform.position + new Vector3(0,1,0)));

            StartCoroutine(wait());
            hitSoundSource.clip = hitSound;
            hitSoundSource.Play();

            // Sends the cops back to their own building to prevent them from stunlocking the player
            foreach (CopHitScript cop in CopSpawner.activeCops)
            {
                cop.ai.target = copBuilding;
            }
            StartCoroutine(wait());
        }

        // After a certain amount of time the Cops will chase the player once more
        IEnumerator wait()
        {
            yield return new WaitForSeconds(copWalkingAwayTimer);
            foreach (CopHitScript cop in CopSpawner.activeCops)
            {
                cop.ai.target = player;
            }
        }
    }
}
