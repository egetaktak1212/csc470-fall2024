using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.EventSystems;


public class Platformer : MonoBehaviour
{
    public CharacterController cc;
    public Transform cameraTransform;
    public GameObject platform;
    public Animator animator;


    float rotateSpeed = 90;
    float moveSpeed = 13f;
    float jumpVelocity;



    // These will be used to simulate gravity, and for jumping
    float yVelocity = 0;
    float gravity;

    // These will be used to create a "dash"
    float dashAmount = 16;
    float dashVelocity = 0;
    float friction = -2.8f;
    float dashTimer = 0;
    float dashLength = .2f;
    int dashCount = 0;  

    // This will keep track of how long we have been falling, we will use this 
    // for "coyote time" (keeping track of how long it has been since we have
    // started falling), and letting the player jump for a certain amount of time.
    float fallingTime = 0;
    float coyoteTime = 0.5f;

    float maxJumpTime = .75f;
    float maxJumpHeight = 4.0f;
    bool calcFallTime = false;
    float otherfalltime = 0f;
    bool isDashing = false;
    bool standingOnMoving = false;
    VelocityCalculator thing;

    bool jumpPad = false;


    // Start is called before the first frame update
    void Start()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    // Update is called once per frame
    void Update()
    {
        //Application.targetFrameRate = 15;
        Cursor.lockState = CursorLockMode.Locked;
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        // --- ROTATION ---
        // Rotate on the y axis based on the hAxis value
        // NOTE: If the player isn't pressing left or right, hAxis will be 0 and there will be no rotation
        //transform.Rotate(0, rotateSpeed * hAxis * Time.deltaTime, 0);

        if (dashTimer == 0)
        {
            isDashing = false;
            dashVelocity = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCount == 0)
        {
            isDashing = true;
            dashVelocity = dashAmount;
            yVelocity = 0;
            dashTimer = dashLength;
            dashCount = 1;
            
        }
        // Slow the dash down, and keep it from going below zero (using clamp)
        dashTimer -= Time.deltaTime;
        dashTimer = Mathf.Clamp(dashTimer, 0, 10000);
        
        

        if (!cc.isGrounded)
        {



            // *** If we are in here, we are IN THE AIR ***
            otherfalltime += Time.deltaTime;
            if (otherfalltime < .25f && !isDashing && (Input.GetKeyDown(KeyCode.Space))) {
                yVelocity = jumpVelocity;
            }

            if (Input.GetKeyDown(KeyCode.Space) && yVelocity < 0.0f && !isDashing)
            {

                calcFallTime = true;
            }

            if (calcFallTime)
            {

                fallingTime += Time.deltaTime;

            }

            // If we go in this block of code, cc.isGrounded is false, which means
            // the last time cc.Move was called, we did not try to enter the ground.

            // If the player releases space and the player is moving upwards, stop upward velocity
            // so that the player begins to fall.
            //if (yVelocity > 0 && Input.GetKeyUp(KeyCode.Space))
            //{
            //    yVelocity = 0;
            //}

            // Apply gravity to the yVelocity
            if (!isDashing)
            {
                if (yVelocity > 0.0f)
                {
                    yVelocity += gravity * Time.deltaTime;
                }
                else if (yVelocity <= 0.0f)
                {
                    yVelocity += gravity * 2.0f * Time.deltaTime;
                }

                if (Input.GetKeyUp(KeyCode.Space)) { yVelocity = 0.0f; }

            }
        }
        else
        {
            otherfalltime = 0f;
            dashCount = 0;
            yVelocity = -2;



            if ((fallingTime < .2f) && calcFallTime)
            {

                yVelocity = jumpVelocity;
            }
            calcFallTime = false;

            // *** If we are in here, we are ON THE GROUND ***

            // Set velocity downward so that the CharacterController collides with the
            // ground again, and isGrounded is set to true.


            fallingTime = 0;

            // Jump!
            if (!isDashing) { 
                if (Input.GetKeyDown(KeyCode.Space))
                {

                    yVelocity = jumpVelocity;
                }
            }
        }

        if (jumpPad)
        {
            yVelocity = jumpVelocity * 2;
            jumpPad = false;
        }

        // --- TRANSLATION ---
        // Move the player forward based on the vAxis value
        // Note, If the player isn't pressing up or down, vAxis will be 0 and there will be no movement
        // based on input. However, yVelocity will still move the player downward.
        //Vector3 amountToMove = transform.forward * moveSpeed * vAxis;

        Vector3 amountToMove = new Vector3(hAxis, 0, vAxis) * moveSpeed;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 forwardRelative = amountToMove.z * camForward;
        Vector3 rightRelative = amountToMove.x * camRight;

        Vector3 moveDir = forwardRelative + rightRelative;

        amountToMove = new Vector3(moveDir.x, 0, moveDir.z);


        // Apply the dash (i.e. add the forward vector scaled by the forwardVelocity)
        amountToMove += amountToMove.normalized * dashVelocity;

        if (!isDashing)
        {
            amountToMove.y += yVelocity;

            
        }
        amountToMove *= Time.deltaTime;
        // This will move the player according to the forward vector and the yVelocity using the
        // CharacterController.

        Vector3 rotate = amountToMove;
        rotate.y = 0;

        if (standingOnMoving)
        {

            amountToMove += thing.GetVelocity() * Time.deltaTime;

        }

        cc.Move(amountToMove);

        if (rotate != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-rotate.normalized), 5f * Time.deltaTime);
        }

        animator.SetBool("isRunning", hAxis != 0 || vAxis != 0);
        animator.SetBool("isIdle", hAxis == 0 && vAxis == 0);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovingPlatform")) {
            Debug.Log("jumped");
            platform = other.gameObject;
            standingOnMoving = true;
            thing = platform.GetComponent<VelocityCalculator>();
            
        }
        if (other.CompareTag("JumpPlatform")) {
           
            jumpPad = true;
        }


    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("MovingPlatform"))
        {
            platform = null;
            standingOnMoving = false;
        }
    }

}