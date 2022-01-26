using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveLocal(gameObject, new Vector3(0f, 50f, 20f), 1).setEase(LeanTweenType.easeOutCubic).destroyOnComplete = true;
    }
}
