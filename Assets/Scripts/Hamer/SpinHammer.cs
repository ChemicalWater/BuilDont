//Code by Eele Roet
using System.Collections;
using UnityEngine;

/// <summary>
/// <para> Handles swinging the hammer with mouse input</para>
/// <para> Plays different states of the animator based on mouse input, plays a sound when the hammer is swung</para> 
/// </summary>
public class SpinHammer : MonoBehaviour
{
    private Animator swingAnimator = null;//animator that moves the hammer
    [SerializeField] private float chargeTime = 0f;//time it takes before the hammer can swing
    [SerializeField] private float hitTime = 0f;//time it takes to swing
    [SerializeField] private Transform hammerTransform = null;//hammer transform used to reset the hammer position after a swing.
    [SerializeField] private AudioSource swing;//audio source that plays the swing sound
 
    //list of bools to keep track of the state of swinging
    private bool charging = false;//hammer can't hit yet
    private bool charged = false;//hammer can now hit
    private bool hitting = false;//hammer is hitting
    private static bool resetHammer = false;//resets hammer rotation next Update cycle
    private bool chargeCoroutineRunning = false;
    private bool hitCoroutineRunning = false;

    private static bool mouseClicked;//handle input next Update cycle
    private static bool staticLeftMouseDown;//saves the information about the mousebutton over frames
    private static bool staticLeftMouseUp;//saves the information about the mousebutton over frames

    private Vector3 idleRotation = new Vector3(45.38f, 40.143f, 0f);//default idle rotation

    // Start is called before the first frame update
    void Start()
    {
        swingAnimator = gameObject.GetComponent<Animator>();
    }


    public void Charge()
    {
        StartCoroutine(StartChargeAnimation(chargeTime));
    }

    IEnumerator StartChargeAnimation(float time)
    {
        chargeCoroutineRunning = true;
        charging = true;
        charged = true;
        swingAnimator.SetBool("charge", true);//animator handles the rotation

        float startTimer = Time.time;
        //skips a frame untill time is up
        while ((Time.time - startTimer) <= time)
        {
            yield return null; 
        }

        charging = false;
        swingAnimator.SetBool("charge", false);//resets the animator boolean
        chargeCoroutineRunning = false;
    }

    private void Hit()
    {
        StartCoroutine(StartHitAnimation(hitTime));
    }
    
    IEnumerator StartHitAnimation(float time)
    {
        hitCoroutineRunning = true;
        //waits untill hammer is no longer charging
        while(charging)
        {
            yield return null;
        }

        charged = false;//uses the charge to hit
        hitting = true;
        swingAnimator.SetBool("hit", true);//animator handles hitting

        yield return new WaitForSeconds(time);//wait untill the animation is over

        hitting = false;
        swingAnimator.SetBool("hit", false);//reset the animator boolean
        hitCoroutineRunning = false;
    }

    public void ResetHamerRotation()
    {
        resetHammer = true; //resets the hammer next Update cycle
    }

    //public method used in HandleWeapons script, the script that handles the input for using weapons.
    public static void CheckForSwing(bool leftMouseDown, bool leftMouseUp)//takes in the information about the mousebuttons when method was called
    {
        mouseClicked = true;//handle input next Update cycle
        staticLeftMouseDown = leftMouseDown;//save the mousebutton information
        staticLeftMouseUp = leftMouseUp;//save the mousebutton information
    }

    // Update is called once per frame
    void Update()
    {
        //handle the input for charging and hitting the hammer
        if (mouseClicked)
        {
            if (staticLeftMouseDown)
            {
                //if the hammer is idling, start the charge
                if (!charging && !hitting && !chargeCoroutineRunning && !hitCoroutineRunning)
                {
                    Charge();
                }
            }
            else if (staticLeftMouseUp)
            {
                swing.Play();//play a swing sound
                //if the hammer is not hitting and it has or is charging, hit the hammer.
                if (!hitting && !hitCoroutineRunning && charged)
                {
                    Hit();
                }
            }
            mouseClicked = false;//reset the bool
        }
        //set the hammer to the default rotation after a swing
        if (resetHammer)
        {
            swingAnimator.Play("Idle");//keep the hammer still
            hammerTransform.localEulerAngles = idleRotation;//reset rotation

            resetHammer = false;//reset the bool
        }
    }
}
