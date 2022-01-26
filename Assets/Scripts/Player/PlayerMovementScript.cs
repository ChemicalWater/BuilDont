using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField]
    AudioSource run;

    //Variables at the top
    Plane playerPlane;
    private float minimumTurnDistance = 0.5f;
    private Rigidbody body;
    public float speed;
    private float vertical;
    private float horizontal;
    private bool playing;
    private static bool knockedOut;
    [SerializeField] private float knockedOutTime;
    private static bool startKnockoutCoroutine;
    private Coroutine knockOutCoroutine;
    private static Vector3 knockDirection;

   [SerializeField] private Animator playerAnim;
 
    //Methods below
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");

        if (!knockedOut)
        {
            // if I am not knocked out yet we can move around
            SetRigidBodyVelocity();
            TurnToMouse();
            SetLegAnimations();
        }
        else
        {
            // if we are knocked out, please help me get out of this
            HandleBeingKnockedOut();
        }
    }

    private void SetRigidBodyVelocity()
    {
        // calculate what direction we are going from where the camera is looking towards
        Vector3 cameraForwardXZ = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        cameraForwardXZ.Normalize();
        Vector3 cameraRightXZ = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);
        cameraRightXZ.Normalize();

        Vector3 moveDirection = (cameraForwardXZ * vertical + cameraRightXZ * horizontal);
        moveDirection.Normalize();

        // move the player with our calculated direction and set speed
        Vector3 newVelocity = moveDirection * speed * Time.fixedDeltaTime;
        newVelocity += new Vector3(0, body.velocity.y, 0);
        body.velocity = newVelocity;
    }

    private void TurnToMouse()
    {
        //updates plane to height of player
        playerPlane.SetNormalAndPosition(Vector3.up, gameObject.transform.position);

        //Create a ray from camera through the mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float enter = 0.0f;

        //get vector of mouse raycast collision with plane of y of playertransform
        if (playerPlane.Raycast(ray, out enter))
        {
            //Get the collision position point
            Vector3 groundIntersection = ray.GetPoint(enter);

            if (Vector3.Distance(transform.position, groundIntersection) > minimumTurnDistance)
            {
                //turn transform forward to collision point
                transform.LookAt(groundIntersection);
            }
        }
    }

    private void SetLegAnimations()
    {
        // check if we are moving
        if (vertical != 0 && horizontal == 0 || vertical == 0 && horizontal != 0)
        {
            // check in what direction we are moving and where the mouse is aiming at
            if (vertical > 0 && Vector3.Angle(transform.forward, Camera.main.transform.forward) <= 45.0 ||
                 horizontal > 0 && Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) <= -45.0 &&
                 Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) >= -135)
            {
                playerAnim.SetBool("walk_forward", true);
                playerAnim.SetBool("walk_backwards", false);
                playerAnim.SetBool("walk_left", false);
                playerAnim.SetBool("walk_right", false);
            }
            // check in what direction we are moving and where the mouse is aiming at
            if (vertical > 0 && Vector3.Angle(transform.forward, Camera.main.transform.forward) >= 135.0 ||
                horizontal > 0 && Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) <= 135.0 &&
                Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) >= 45)
            {
                playerAnim.SetBool("walk_forward", false);
                playerAnim.SetBool("walk_backwards", true);
                playerAnim.SetBool("walk_left", false);
                playerAnim.SetBool("walk_right", false);
            }
            // check in what direction we are moving and where the mouse is aiming at
            if (vertical < 0 && Vector3.Angle(transform.forward, Camera.main.transform.forward) >= 135.0 ||
                 horizontal < 0 && Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) <= 135.0 &&
                 Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) >= 45.0)

            {
                playerAnim.SetBool("walk_forward", true);
                playerAnim.SetBool("walk_backwards", false);
                playerAnim.SetBool("walk_left", false);
                playerAnim.SetBool("walk_right", false);
            }
            // check in what direction we are moving and where the mouse is aiming at
            if (vertical < 0 && Vector3.Angle(transform.forward, Camera.main.transform.forward) <= 45.0 ||
                horizontal < 0 && Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) >= -135.0 &&
                Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) <= -45)
            {
                playerAnim.SetBool("walk_forward", false);
                playerAnim.SetBool("walk_backwards", true);
                playerAnim.SetBool("walk_left", false);
                playerAnim.SetBool("walk_right", false);
            }
            // check in what direction we are moving and where the mouse is aiming at
            if (horizontal < 0 && Vector3.Angle(transform.forward, Camera.main.transform.forward) <= 45.0 ||
                vertical > 0 && Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) >= -135.0 &&
                Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) <= -45 ||
                horizontal > 0 && Vector3.Angle(transform.forward, Camera.main.transform.forward) >= 135.0 ||
                vertical < 0 && Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) <= 135.0 &&
                Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) >= 45)
            {
                playerAnim.SetBool("walk_left", true);
                playerAnim.SetBool("walk_right", false);
                playerAnim.SetBool("walk_backwards", false);
                playerAnim.SetBool("walk_forward", false);
            }
            // check in what direction we are moving and where the mouse is aiming at
            if (vertical > 0 && Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) <= 135.0 &&
                 Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) >= 45.0 ||
                 vertical < 0 && Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) >= -135.0 &&
                Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, Vector3.up) <= -45 ||
                horizontal > 0 && Vector3.Angle(transform.forward, Camera.main.transform.forward) <= 45.0 ||
                horizontal < 0 && Vector3.Angle(transform.forward, Camera.main.transform.forward) >= 135.0)

            {
                playerAnim.SetBool("walk_left", false);
                playerAnim.SetBool("walk_right", true);
                playerAnim.SetBool("walk_backwards", false);
                playerAnim.SetBool("walk_forward", false);
            }
        }
        // if the player is playing but we are not moving, turn off all animations but only idle
        else if (body.velocity.magnitude <= 0.1f && playing == true)
        {
            playerAnim.SetBool("idle", true);
            playerAnim.SetBool("walk_left", false);
            playerAnim.SetBool("walk_right", false);
            playerAnim.SetBool("walk_forward", false);
            playerAnim.SetBool("walk_backwards", false);
            run.Pause();
            playing = false;
        }
        // if the player started to move again, turn idle off
        if (body.velocity.magnitude >= 0.1f && playing == false)
        {
            playerAnim.SetBool("idle", false);
            run.Play();
            playing = true;
        }
    }

    private void HandleBeingKnockedOut()
    {
        // if the player is knocked out, stop the coroutine, start the wait to turn off ragdol and knock the player away
        if(startKnockoutCoroutine)
        {
            if (knockOutCoroutine != null)
            {
                StopCoroutine(knockOutCoroutine);
            }
            knockOutCoroutine = StartCoroutine(TurnOffRagdollAfterSeconds(knockedOutTime));
            startKnockoutCoroutine = false;
            body.AddForce(knockDirection * 30000);
        }
    }

    public static void Ragdoll(Vector3 direction)
    {
        // We are being knocked out, we can start the coroutine and the direction we are knocked from is set
        knockedOut = true;
        startKnockoutCoroutine = true;
        knockDirection = direction;
        knockDirection.Normalize();
    }

    private IEnumerator TurnOffRagdollAfterSeconds(float seconds)
    {
        // turn off the bodyconstraints and go back to idle
        body.constraints = RigidbodyConstraints.None;
        playerAnim.SetBool("idle", true);
        playerAnim.SetBool("walk_left", false);
        playerAnim.SetBool("walk_right", false);
        playerAnim.SetBool("walk_forward", false);
        playerAnim.SetBool("walk_backwards", false);

        run.Pause();
        playing = false;

        // wait a bit and turn knocked off and freeze our limbs rotations
        yield return new WaitForSeconds(seconds);
        knockedOut = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;
    }

}
