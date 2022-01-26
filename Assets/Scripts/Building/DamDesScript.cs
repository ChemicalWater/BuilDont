using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamDesScript : MonoBehaviour
{
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private Animation damAnime;
    [SerializeField] private GameObject watar;
    [SerializeField] private Animation watarAnime;
    [SerializeField] DisableCameraZones disableZones;

    /// <summary>
    /// Plays a set of animations for when the dam is destroyed.
    /// </summary>
    public void DestroyDam()
    {
        damAnime.Play();
        watar.SetActive(true);
        watarAnime.Play();
        disableZones.disableFocusZones();

    }
}
