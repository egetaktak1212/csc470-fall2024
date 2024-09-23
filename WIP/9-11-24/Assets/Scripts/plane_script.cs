using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class plane_script : MonoBehaviour
{
    public GameObject cameraObject;
    public GameObject rotationObjectY;
    public GameObject rotationObjectX;

    float xRotationSpeed = 120f;
    float zRotationSpeed = 120f;
    float yRotationSpeed = 100f;
    public float forwardSpeed = 5f;
    bool boost = false;
    float Timer = 0;
    float time;
    int booster = 1;
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
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = -Input.GetAxis("Horizontal"); // -1 if left is pressed, 1 if right is pressed, 0 if neither
        float vAxis = Input.GetAxis("Vertical"); // -1 if down is pressed, 1 if up is pressed, 0 if neither
        float yAxis = Input.GetAxis("Yaw");

        // Apply the rotation based on the inputs
        Vector3 amountToRotate = new Vector3(0, 0, 0);
        amountToRotate.x = vAxis * xRotationSpeed;
        amountToRotate.z = hAxis * zRotationSpeed;
        amountToRotate.y = yAxis * yRotationSpeed;
        amountToRotate *= Time.deltaTime; // amountToRotate = amountToRotate * Time.deltaTime;



        //GRADUAL MOVEMENT CODE STARTS HERE, TAKE TYLENOL

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
                slowPower = 0;
                if (gradRotate < 1)
                {
                    gradRotate += 0.7f * Time.deltaTime;
                }
            }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            gradRotate = 0;
            if (slowDownBool == 1)
            { //if slowdownbool is 1, that means we were going right. this means that slowDown is going down
                if (slowDown > 0)
                {
                    slowDown += -slowDownBool * 0.06f * Time.deltaTime * Mathf.Pow(2, slowPower);
                    slowPower += .01f;
                }
                else
                {
                    slowDown = 0;
                }
            }
            else if (slowDownBool == -1) { //slowdownbool negative, we goin left, this means slowDown is goin up
                if (slowDown < 0)
                {
                    slowDown += -slowDownBool * 0.06f * Time.deltaTime * Mathf.Pow(2, slowPower);
                    slowPower += .01f;
                }
                else {
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
            slowDownBig = -slowDownBoolBig;
        }

        if (vAxis == 1)
        {
            slowDownBig = slowDownBoolBig;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            slowPowerBig = 0;
            if (gradRotateBig < 1)
            {
                gradRotateBig += 0.7f * Time.deltaTime;
            }
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            gradRotateBig = 0;
            if (slowDownBoolBig == 1)
            { //if slowdownboolbig is 1, that means we were going up. this means that slowDownBig is going down
                if (slowDownBig > 0)
                {
                    slowDownBig += -slowDownBoolBig * 0.06f * Time.deltaTime * Mathf.Pow(2, slowPowerBig);
                    slowPowerBig += .01f;
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
                    slowDownBig += -slowDownBoolBig * 0.06f * Time.deltaTime * Mathf.Pow(2, slowPowerBig);
                    slowPowerBig += .01f;
                }
                else
                {
                    slowDownBig = 0;
                }

            }

        }

        //END OF GRAD MOVEMENT CODE


        transform.RotateAround(transform.position, Vector3.up, (amountToRotate.y * gradRotate) + slowDown);
        transform.Rotate((amountToRotate.x),0,0, Space.Self);

        //transform.Rotate(amountToRotate, Space.Self);

        
        transform.position += transform.forward * Time.deltaTime * forwardSpeed * booster;
        //print(booster);
        Vector3 camPos = transform.position;
        camPos += -transform.forward * 6f;
        camPos += Vector3.up * 4f;
        cameraObject.transform.position = camPos;

        cameraObject.transform.LookAt(transform.position);


        Timer += Time.deltaTime;
        Debug.Log(vAxis);

    }

    private void OnTriggerEnter(Collider other)
    {
        print("collision");
        time = Timer;
        if (other.CompareTag("Booster"))
        {
            Boost();
        }
    }

    public void Boost() {
        booster = 2;
    }


}
