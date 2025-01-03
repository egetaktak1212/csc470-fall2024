using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using TMPro;

public class plane_script : MonoBehaviour
{

    public GameObject cameraObject;
    public GameObject rotationObjectY;
    public GameObject rotationObjectX;
    public GameObject plane;
    public TMP_Text juiceMeter;


    static int score = 0;

    float variable = 0;

    float xRotationSpeed = 120f;
    float zRotationSpeed = 120f;
    float yRotationSpeed = 100f;
    float forwardSpeedMult;
    static float forwardSpeed = 60f;
    static bool boost = false;
    static float boostTimer = 0;
    float Timer = 0;
    float time;
    static int booster = 1;
    float gradRotate;
    float gradRotateBig;
    float slowDown;
    float slowDownBool;
    float slowDownSpeed = 0.05f;
    float slowPower;
    float slowDownBig;
    float slowDownBoolBig;
    float slowPowerBig;
    float slowDownSpeedBig = 0.05f;
    float gradPower;
    float gradPowerBig;
    float turnDeltaCalc;
    float turnAmount;
    float wingSpeedD = 0;
    float wingSpeedA = 0;
    float wingSpeedNoneD = 0;
    float wingSpeedNoneA = 0;
    float wingSpeedPowerA = 0;
    float wingSpeedPowerD = 0;
    float boostFOV = 0;
    float superBoostFOV = 0;
    static float superBoostTimer;
    static bool superBoost;
    float superBooster = 1;


    private void OnLevelWasLoaded(int level)
    {
        superBoostTimer = 0;
        superBoost = false;
        booster = 1;
        boostTimer = 0;
        boost = false;
        booster = 1;
        score = 0;
        forwardSpeed = 60;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P)) {
            valueChanger();
        }

        Debug.Log(variable);

        //speed decreases gradually (this was a requirement, please see it, i only did it for the requirement)
        //if (forwardSpeed > 0)
        //{
        //    forwardSpeed -= 1f * Time.deltaTime;
        //}
        juiceMeter.text = "JUICE METER: " + score;

        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(0);

        }


        float hAxis = -Input.GetAxis("Horizontal"); // -1 if left is pressed, 1 if right is pressed, 0 if neither
        float vAxis = Input.GetAxis("Vertical"); // -1 if down is pressed, 1 if up is pressed, 0 if neither
        float yAxis = Input.GetAxis("Yaw");

        // Apply the rotation based on the inputs
        Vector3 amountToRotate = new Vector3(0, 0, 0);
        amountToRotate.x = vAxis * xRotationSpeed;
        amountToRotate.z = hAxis * zRotationSpeed;
        amountToRotate.y = yAxis * yRotationSpeed;
        amountToRotate *= Time.deltaTime; // amountToRotate = amountToRotate * Time.deltaTime;

        float Rotation;
        if (plane.transform.eulerAngles.z <= 180f)
        {
            Rotation = plane.transform.eulerAngles.z;
        }
        else
        {
            Rotation = plane.transform.eulerAngles.z - 360f;
        }

        float RotationX;
        if (transform.eulerAngles.x <= 180f)
        {
            RotationX = transform.eulerAngles.x;
        }
        else
        {
            RotationX = transform.eulerAngles.x - 360f;
        }


        //GRADUAL MOVEMENT CODE STARTS HERE, TAKE TYLENOL
        //ps i put a if statement here so u can minimize it by pressing the arrow on the left

        if (true)
        {

            /*
             * Ok so I'm quite proud of what's going on here and will definitely forget the second I look away for 15 minutes. I gotta write this here so I understand later
             * I feel like I could've found a tutorial for this but here we are:
             * the goal was to have the plane accelerate when you pressed A or D until max acc. I didn't want it to snap into the max speed immediately.
             * To do this, I used grad rotate. GradRotate, when key is pressed, starts at 0 and increases until it's 1. Why does it increase? It's because below,
             * the amount to rotate is being multiplied by grad rotate. So, unless gradrotate is 1, it wont go at max speed. When the key is released, gradrotate is set back to 0 to
             * prepare for the next run. Thats the simple one
             * slowDown is my pride and joy. I didn't want the plane to stop immediately so I have slowDown basically decrease the plane's speed exponentially once the key is let go.
             * slowDown is a value being added to the rotation. So, the higher slowDown's absolute value is, the faster it will turn. so, if amount to rotate is 0, slowdown will still 
             * make the plane turn because it's adding on top. First, slowDown needs to be negative if plane is going left and positive if plane is going right. If it was always positive, 
             * plane would turn right upon releasing left. Here, I check if yAxis is -1 or 1, where I set slowDown to pos or negative depending on that.
             * Here's the meat of the code. If A and D are both released, we initiate. slowDownBool decides if slowdown should be decreasing or increasing (if we're going left, slowdown should be increasing to bring us to 0)
             * Inside of each statement, we have math that increases or decreases slowDown until it goes from neg to pos or pos to neg. The math there is fun. It's multiplying by the opposite of slowDownBool to tell the code
             * whether we going up or down. I mult by some constant float, deltaTime for the time thing, and 2 to the power of slow power. Acceleration isn't linear, i think. So i thought it would be cool to make it exponential.
             * As the code runs, slow power increases to make the speed decrease faster. makes sense. once key is pressed again, slowPower goes to 0, and everything is back to normal.
             * */

            if (yAxis == -1 || yAxis == 1)
            {
                slowDownBool = yAxis;
            }

            if (yAxis == -1)
            {
                slowDown = -slowDownSpeed;
            }

            if (yAxis == 1)
            {
                slowDown = slowDownSpeed;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                slowPower = 1;
                if (gradRotate < 1)
                {
                    gradRotate += 1f * Time.deltaTime;
                    //* Mathf.Pow(2, gradPower);
                    //gradPower += 5f * Time.deltaTime;
                }
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                gradRotate = 0;
                gradPower = 0;
            }
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                gradRotate = 0;
                if (slowDownBool == 1)
                { //if slowdownbool is 1, that means we were going right. this means that slowDown is going down
                    if (slowDown > 0)
                    {
                        slowDown += -slowDownBool * 0.1f * Time.deltaTime * Mathf.Pow(2, slowPower);
                        slowPower += 1f * Time.deltaTime;
                    }
                    else
                    {
                        slowDown = 0;
                    }
                }
                else if (slowDownBool == -1)
                { //slowdownbool negative, we goin left, this means slowDown is goin up
                    if (slowDown < 0)
                    {
                        slowDown += -slowDownBool * 0.1f * Time.deltaTime * Mathf.Pow(2, slowPower);
                        slowPower += 1f * Time.deltaTime;
                    }
                    else
                    {
                        slowDown = 0;
                    }

                }

            }
            //END OF GRADUAL MOVEMENT CODE FOR HORIZONTAL MOVEMENT AAAAAAAAAAAAAAAAA

            //START OF GRADUAL MOVEMENT CODE FOR VERTICAL MOVEMENT
            if (vAxis == -1 || vAxis == 1)
            {
                slowDownBoolBig = vAxis;
            }

            if (vAxis == -1)
            {
                slowDownBig = -slowDownSpeedBig;
            }

            if (vAxis == 1)
            {
                slowDownBig = slowDownSpeedBig;
            }

            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                gradRotateBig = 0;
                gradPowerBig = 0;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                slowPowerBig = 1;
                if (gradRotateBig < 1)
                {
                    gradRotateBig += 0.9f * Time.deltaTime;
                    //* Mathf.Pow(2, gradPowerBig);
                    //gradPowerBig += 0.5f * Time.deltaTime;
                }
            }

            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                gradRotateBig = 0;
                if (slowDownBoolBig == 1)
                { //if slowdownboolbig is 1, that means we were going up. this means that slowDownBig is going down
                    if (slowDownBig > 0)
                    {
                        slowDownBig += -slowDownBoolBig * 0.03f * Time.deltaTime * Mathf.Pow(2, slowPowerBig);
                        slowPowerBig += .1f * Time.deltaTime;
                    }
                    else
                    {
                        slowDownBig = 0;
                    }
                }
                else if (slowDownBoolBig == -1)
                { //slowdownboolBig negative, we goin down, this means slowDownBig is goin up
                    if (slowDownBig < 0)
                    {
                        slowDownBig += -slowDownBoolBig * 0.03f * Time.deltaTime * Mathf.Pow(2, slowPowerBig);
                        slowPowerBig += .1f * Time.deltaTime;
                    }
                    else
                    {
                        slowDownBig = 0;
                    }

                }

            }
        }

        //END OF GRAD MOVEMENT CODE

        //Rotates usign its position as the axis. It's the same as having an empty parented
        turnAmount = (2 * (amountToRotate.y * gradRotate) + slowDown);
        transform.RotateAround(transform.position, Vector3.up, turnAmount);
        //find out max and min turn amount, turn it into an angle. max is 180, min is 0. turn based off that

        float verticalMagic = (amountToRotate.x * gradRotateBig) + slowDownBig;
        //negative means up


        //this stops the plane from flipping by going up too much
        if (verticalMagic > 0)
        {
            if (RotationX < 80)
            {
                transform.Rotate(verticalMagic, 0, 0, Space.Self);
            }
        }

        if (verticalMagic < 0)
        {
            if (RotationX > -80)
            {
                transform.Rotate(verticalMagic, 0, 0, Space.Self);
            }
        }

        //plane is an empty that controls the plane mesh. so i just rotate the plane mesh without affecting the movement of the actual plane. the plane can rotate however it wants now

        //boost mechanic
        if (boost)
        {
            if (boostTimer < 7)
            {
                booster = 3;
                boostTimer += Time.deltaTime;
                if (boostFOV < 20)
                {
                    boostFOV += 50 * Time.deltaTime;
                }
            }
            else if (boostTimer >= 7)
            {


                booster = 1;
                boostTimer = 0;
                boost = false;
            }
        }
        if (boostTimer == 0)
        {
            if (boostFOV > 0)
            {
                boostFOV -= 20 * Time.deltaTime;

            }
        }

        if (superBoost)
        {
            if (superBoostTimer < 20)
            {
                superBooster = 10;
                superBoostTimer += Time.deltaTime;
                if (superBoostFOV < 50)
                {
                    superBoostFOV += 120 * Time.deltaTime;
                }
            }
            else if (superBoostTimer >= 20)
            {


                superBooster = 1;
                superBoostTimer = 0;
                superBoost = false;
            }
        }
        if (superBoostTimer == 0)
        {
            if (superBoostFOV > 0)
            {
                superBoostFOV -= 120 * Time.deltaTime;

            }
        }


        forwardSpeedMult = RotationX * 0.1f;

        transform.position += transform.forward * Time.deltaTime * (forwardSpeed + forwardSpeedMult) * booster * superBooster;

        //code to make the wings turn when plane is turning
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            if (turnAmount > 0)
            {
                wingSpeedA = 0;
                if (Rotation > -30f && Rotation < 30.5f)
                {
                    plane.transform.Rotate(0, 0, wingSpeedD * 900 * Time.deltaTime, Space.Self);
                    wingSpeedD -= .2f * Time.deltaTime;
                }
            }
            else if (turnAmount < 0)
            {
                wingSpeedD = 0;
                if (Rotation > -30.5f && Rotation < 30f)
                {
                    plane.transform.Rotate(0, 0, wingSpeedA * 900 * Time.deltaTime, Space.Self);
                    wingSpeedA += .2f * Time.deltaTime;
                }
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            wingSpeedD = 0;
            wingSpeedNoneD = 0;
            wingSpeedNoneA = 0;
            wingSpeedPowerD = 1;
            wingSpeedPowerA = 1;
            if (Rotation > -33f && Rotation < 30f)
            {
                plane.transform.Rotate(0, 0, wingSpeedA * 900 * Time.deltaTime, Space.Self);
                wingSpeedA += .2f * Time.deltaTime;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            wingSpeedA = 0;
            wingSpeedNoneD = 0;
            wingSpeedNoneA = 0;
            wingSpeedPowerD = 1;
            wingSpeedPowerA = 1;
            if (Rotation > -30f && Rotation < 33f)
            {
                plane.transform.Rotate(0, 0, wingSpeedD * 900 * Time.deltaTime, Space.Self);
                wingSpeedD -= .2f * Time.deltaTime;
            }
        }

        if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            wingSpeedA = 0;
            wingSpeedD = 0;
            if (Rotation > 0.5)
            {

                plane.transform.Rotate(0, 0, wingSpeedNoneA * 900 * Time.deltaTime, Space.Self);
                wingSpeedNoneA -= .5f * Time.deltaTime * wingSpeedPowerA;
                wingSpeedPowerA /= 1.005f;
            }
            if (Rotation < -0.5)
            {

                plane.transform.Rotate(0, 0, wingSpeedNoneD * 900 * Time.deltaTime, Space.Self);
                wingSpeedNoneD += .5f * Time.deltaTime * wingSpeedPowerD;
                wingSpeedPowerD /= 1.005f;
            }

        }



        
        //forwardSpeed = 20f + forwardSpeedMult;


        Camera.main.fieldOfView = 60 + forwardSpeed * 1f + boostFOV + superBoostFOV;

        //cam controls
        Vector3 camPos = transform.position;
        camPos += -transform.forward * 6f;
        camPos += Vector3.up * 2f;
        cameraObject.transform.position = camPos;
        Vector3 thing = new Vector3(0, 1, 0);
        cameraObject.transform.LookAt(transform.position + thing);



        Timer += Time.deltaTime;
        //Debug.Log(wingSpeedA + "    " + wingSpeedD + "     "+ Rotation + "         " + turnAmount);
        //Debug.Log(forwardSpeed);

    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Booster"))
        {
            boostTimer = 0;
            boost = true;

        }

        if (other.CompareTag("SuperBoost"))
        {
            superBoostTimer = 0;
            superBoost = true;

        }

        if (other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
            score++;
            //forwardSpeed = 20;

        }
        if (other.CompareTag("Terrain"))
        {
            //Debug.Log("collided");
            SceneManager.LoadScene("SampleScene");
        }



    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            //Debug.Log("collided");
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void valueChanger() {
        variable = 2;
    }


}
