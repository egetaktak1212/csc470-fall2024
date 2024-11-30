using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.EventSystems;


public class Platformer : MonoBehaviour
{
    public CharacterController cc;
    public Transform cameraTransform;
    public GameObject platform;
    public Animator animator;
    public GameObject respawnObj;
    public TMP_Text youwin;



    float rotateSpeed = 90;
    float moveSpeed = 13f;
    float jumpVelocity;




    float yVelocity = 0;
    float gravity;


    float dashAmount = 32;
    float dashVelocity = 0;
    float friction = -2.8f;
    float dashTimer = 0;
    float dashLength = .2f;
    int dashCount = 0;  

    //if you press jump before u land, it'll make u jump when u touch ground
    float fallingTime = 0;

    //coyote
    float coyoteTime = 0.5f;

    float maxJumpTime = .90f;
    float maxJumpHeight = 6.0f;
    bool calcFallTime = false;
    float otherfalltime = 0f;
    bool isDashing = false;
    bool standingOnMoving = false;
    VelocityCalculator thing;

    bool jumpPad = false;
    int jumpCount = 0;

    bool dead = false;

    Vector3 prevPlat;


    // Start is called before the first frame update
    void Start()
    {
        youwin.enabled = false;
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

        if (dashTimer == 0 || (isDashing && Input.GetKeyUp(KeyCode.LeftShift)))
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
        //dash isnt friction based (yuck!)
        dashTimer -= Time.deltaTime;
        dashTimer = Mathf.Clamp(dashTimer, 0, 10000);
        
        

        if (!cc.isGrounded)
        {
            // *** If we are in here, we are IN THE AIR ***

            otherfalltime += Time.deltaTime;
            if (!isDashing && Input.GetKeyDown(KeyCode.Space) && jumpCount == 0 && otherfalltime > .25f) { 
                yVelocity = jumpVelocity;
                jumpCount++;
                animator.SetTrigger("Backflip");
            }


            
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

                //if (Input.GetKeyUp(KeyCode.Space) && yVelocity > 0) { yVelocity = 0.0f; }

            }
        }
        else if (cc.isGrounded || standingOnMoving) 
        {
            otherfalltime = 0f;
            dashCount = 0;

            if (!standingOnMoving)
            {
                yVelocity = -2;
            }
            jumpCount = 0;



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


        ////UNCOMMENT IF GIVE UP ON VELCITY
        //if (standingOnMoving)
        //{

        //    amountToMove += thing.GetVelocity() * Time.deltaTime;

        //}

        animator.SetBool("IsRunning", hAxis != 0 || vAxis != 0);
        animator.SetBool("IsIdle", hAxis == 0 && vAxis == 0);
        bool a = animator.GetBool("IsIdle");
        bool b = animator.GetBool("IsRunning");



        if (platform != null)
        {
            Vector3 amountPlatformMoved = platform.transform.position - prevPlat;
            amountToMove += amountPlatformMoved;
            prevPlat = platform.transform.position;
        }




        cc.Move(amountToMove);

        if (rotate != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-rotate.normalized), 5f * Time.deltaTime);
        }
        if (dead)
        {
            cc.enabled = false;
            transform.position = respawnObj.transform.position;
            dead = false;
            cc.enabled = true; 
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovingPlatform")) {
            //platform = other.gameObject;
            standingOnMoving = true;
            //thing = platform.GetComponent<VelocityCalculator>();

            platform = other.gameObject;
            prevPlat = platform.transform.position;

            
        }
        if (other.CompareTag("JumpPlatform")) {
           
            jumpPad = true;
        }

        if (other.CompareTag("Respawn")) {
            dead = true;
        
        }

        if (other.CompareTag("Win"))
        {
            Debug.Log("win");
            youwin.enabled = true;


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