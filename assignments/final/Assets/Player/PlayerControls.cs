using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static PlayerControls;


public class PlayerControls : MonoBehaviour
{
    public CharacterController cc;
    public Transform cameraTransform;
    public Action PreUpdate;
    public Action<Vector3, float> PostUpdate;
    public GameObject damageTextPrefab;
    public GameObject startPosition;
    public GameObject checkpointPosition;
    Vector3 checkpointPos;
    bool respawn = false;

    bool Strafe = false;
    public void SetStrafeMode(bool b) => Strafe = b;
    public bool isMoving = false;
    public HealthBarScript healthbar;
    public int maxHealth = 100;
    public float currentHealth;
    float timeH = 0f;
    public bool recoverH = true;
    public int publicDamage = 0;

    float moveSpeed = 13f;
    float jumpVelocity;

    float yVelocity = 0;
    float gravity;

    //if you press jump before u land, it'll make u jump when u touch ground
    float fallingTime = 0;

    float maxJumpTime = .70f;
    float maxJumpHeight = 4.0f;
    bool calcFallTime = false;
    float otherfalltime = 0f;

    int jumpCount = 0;

    public CameraStyle currentStyle;

    public Transform combatLookAt;

    //dash
    float dashAmount = 32;
    float dashVelocity = 0;
    float dashTimer = 0;
    float dashLength = .2f;
    int dashCount = 0;
    int groundDashCount = 0;
    bool isDashing = false;
    bool canDash = true;
    public int maxDashes = 1;
    public int maxJumps = 2;

    public enum CameraStyle
    {
        Open,
        Combat
    }
    //public GameObject openCamera;
    //public GameObject adsCamera;



    // Start is called before the first frame update
    void Start()
    {
        checkpointPos = startPosition.transform.position;


        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = (2 * maxJumpHeight) / timeToApex;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        //SwitchCamera(CameraStyle.Open);
        currentHealth = maxHealth; // Initialize health
        healthbar.setHealth(currentHealth); // Update health bar
    }

    // Update is called once per frame
    void Update()
    {
        if (!recoverH)
        {
            timeH = 10f;
            currentHealth -= publicDamage;
        }
        if (timeH <= 0)
        {
            // Health recovery
            if ((currentHealth + 2) > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += 4f * Time.deltaTime;
            }
        }
        publicDamage = 0;
        recoverH = true;
        timeH -= Time.deltaTime;
        healthbar.setHealth(currentHealth);


        PreUpdate?.Invoke();
        bool dashedThisTurn = false;

        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");


        if (dashTimer == 0 /*|| (isDashing && Input.GetKeyUp(KeyCode.LeftShift))*/)
        {
            isDashing = false;
            dashVelocity = 0;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCount < maxDashes && groundDashCount < maxDashes && canDash)
        {
            if (maxDashes != 2)
            {
                dashedThisTurn = true;
            }
            else if (groundDashCount == 1)
            {
                dashedThisTurn = true;
            }
            isDashing = true;
            dashVelocity = dashAmount;
            yVelocity = 0;
            dashTimer = dashLength;
            if (cc.isGrounded)
            {
                groundDashCount++;
            }
            else
            {
                dashCount++;
            }
        }
        dashTimer -= Time.deltaTime;
        dashTimer = Mathf.Clamp(dashTimer, 0, 10000);

        if (!cc.isGrounded)
        {
            //Debug.Log("in the air for some reason");

            // *** If we are in here, we are IN THE AIR ***

            otherfalltime += Time.deltaTime;
            if (!isDashing && Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
            {
                yVelocity = jumpVelocity;
                jumpCount++;
            }



            if (otherfalltime < .25f && !isDashing && jumpCount == 0 && (Input.GetKeyDown(KeyCode.Space)))
            {
                yVelocity = jumpVelocity;
                jumpCount++;
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
            }

            //if (Input.GetKeyUp(KeyCode.Space) && yVelocity > 0) { yVelocity = 0.0f; }


        }
        else if (cc.isGrounded)
        {
            otherfalltime = 0f;
            dashCount = 0;


            yVelocity = -2;
            jumpCount = 0;

            //this is to add a delay when trying to dash on the ground
            if (dashedThisTurn)
            {
                StartCoroutine(DisableDash());
            }


            if ((fallingTime < .2f) && calcFallTime)
            {
                jumpCount++;
                yVelocity = jumpVelocity;
            }
            calcFallTime = false;
            fallingTime = 0;

            // Jump!
            if (!isDashing)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    groundDashCount = 0;
                    jumpCount++;
                    yVelocity = jumpVelocity;
                }
            }

        }




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

        if (amountToMove != Vector3.zero)
        {
            amountToMove += amountToMove.normalized * dashVelocity;
        }
        else
        {
            amountToMove += transform.forward * dashVelocity;
        }
        if (!isDashing)
        {
            amountToMove.y += yVelocity;


        }

        amountToMove *= Time.deltaTime;

        isMoving = amountToMove != Vector3.zero;



        //animator.SetBool("IsRunning", hAxis != 0 || vAxis != 0);
        //animator.SetBool("IsIdle", hAxis == 0 && vAxis == 0);
        //bool a = animator.GetBool("IsIdle");
        //bool b = animator.GetBool("IsRunning");



        cc.Move(amountToMove);


        if (!Strafe)
        {
            Vector3 rotate = amountToMove;
            rotate.y = 0;
            if (rotate != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotate.normalized), 5f * Time.deltaTime);
            }
        }

        if (respawn)
        {
            cc.enabled = false;
            respawn = false;
            transform.position = checkpointPos;
            cc.enabled = true;
        }

        if (PostUpdate != null)
        {

            PostUpdate(Vector3.zero, false ? 1 : 1);
        }



    }


    IEnumerator DisableDash()
    {
        canDash = false;
        yield return new WaitForSeconds(0.5f);
        canDash = true;
        groundDashCount = 0;
    }
    public void TakeDamage(int damage)
    {
        recoverH = false;
        currentHealth -= damage;
        healthbar.setHealth(currentHealth);

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
        GameObject DamageText = Instantiate(damageTextPrefab, pos, Quaternion.identity);
        DamageText.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = damage.ToString();
        if (currentHealth <= 0)
        {
            currentHealth = 0; // Prevent negative health
                               // Optionally disable player controls or trigger a death animation

            GM.instance.killPlayer();

            // Destroy the player object after ending any active logic
            Destroy(gameObject);




        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckpointSave"))
        {
            checkpointPos = checkpointPosition.transform.position;

        }
    }
    public void TPtoCheckpoint()
    {

        respawn = true;
    }



}