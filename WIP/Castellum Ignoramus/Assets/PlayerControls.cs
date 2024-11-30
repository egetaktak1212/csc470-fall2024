using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static PlayerControls;


public class PlayerControls : MonoBehaviour
{
    public CharacterController cc;
    public Transform cameraTransform;

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

    public enum CameraStyle
    {
        Open,
        Combat
    }
    public GameObject openCamera;
    public GameObject adsCamera;



    // Start is called before the first frame update
    void Start()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = (2 * maxJumpHeight) / timeToApex;
        SwitchCamera(CameraStyle.Open);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("A");
            SwitchCamera(CameraStyle.Combat);
            Debug.Log("B");
        }
        if (Input.GetMouseButtonUp(1))
        {
            SwitchCamera(CameraStyle.Open);
            Debug.Log(currentStyle + " ");
        }




        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");


        if (!cc.isGrounded)
        {
            // *** If we are in here, we are IN THE AIR ***

            otherfalltime += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount == 0 && otherfalltime > .25f)
            {
                yVelocity = jumpVelocity;
                jumpCount++;
            }



            if (otherfalltime < .25f && (Input.GetKeyDown(KeyCode.Space)))
            {
                yVelocity = jumpVelocity;
            }

            if (Input.GetKeyDown(KeyCode.Space) && yVelocity < 0.0f)
            {

                calcFallTime = true;
            }

            if (calcFallTime)
            {

                fallingTime += Time.deltaTime;

            }


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
        else if (cc.isGrounded)
        {
            otherfalltime = 0f;


            yVelocity = -2;
            jumpCount = 0;



            if ((fallingTime < .2f) && calcFallTime)
            {

                yVelocity = jumpVelocity;
            }
            calcFallTime = false;
            fallingTime = 0;

            // Jump!
            if (Input.GetKeyDown(KeyCode.Space))
            {

                yVelocity = jumpVelocity;
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

        amountToMove.y += yVelocity;


        amountToMove *= Time.deltaTime;



        //animator.SetBool("IsRunning", hAxis != 0 || vAxis != 0);
        //animator.SetBool("IsIdle", hAxis == 0 && vAxis == 0);
        //bool a = animator.GetBool("IsIdle");
        //bool b = animator.GetBool("IsRunning");



        cc.Move(amountToMove);

        if (currentStyle == CameraStyle.Open)
        {
            Vector3 rotate = amountToMove;
            rotate.y = 0;
            if (rotate != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotate.normalized), 5f * Time.deltaTime);
            }
        }
        else if (currentStyle == CameraStyle.Combat)
        {

            Vector3 dirToLook = combatLookAt.position - new Vector3(cameraTransform.position.x, combatLookAt.position.y, cameraTransform.position.z);
            transform.forward = dirToLook.normalized;

        }




    }

    void SwitchCamera(CameraStyle cameraStyle)
    {
        Debug.Log("C");

        Vector3 openTransform = openCamera.transform.position;
        Quaternion openRotation = openCamera.transform.rotation;

        Vector3 combatTransform = adsCamera.transform.position;
        Quaternion combatRotation = adsCamera.transform.rotation;

        if (cameraStyle == CameraStyle.Open)
        {
            openCamera.SetActive(true);
            adsCamera.SetActive(false);

            openCamera.transform.position = combatTransform;
            openCamera.transform.rotation = combatRotation;
        }
        else if (cameraStyle == CameraStyle.Combat) {
            adsCamera.SetActive(true);
            openCamera.SetActive(false);

            adsCamera.transform.position = openTransform;
            adsCamera.transform.rotation = openRotation;
        }



        currentStyle = cameraStyle;

    }



}