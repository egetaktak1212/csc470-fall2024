using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class plane_script : MonoBehaviour
{
    public GameObject cameraObject;

    float xRotationSpeed = 90f;
    float yRotationSpeed = 90f;
    public float forwardSpeed = 0.2f;
    bool boost = false;
    float Timer;
    float time;
    int booster = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal"); // -1 if left is pressed, 1 if right is pressed, 0 if neither
        float vAxis = Input.GetAxis("Vertical"); // -1 if down is pressed, 1 if up is pressed, 0 if neither

        // Apply the rotation based on the inputs
        Vector3 amountToRotate = new Vector3(0, 0, 0);
        amountToRotate.x = vAxis * xRotationSpeed;
        amountToRotate.y = hAxis * yRotationSpeed;
        amountToRotate *= Time.deltaTime; // amountToRotate = amountToRotate * Time.deltaTime;
        transform.Rotate(amountToRotate, Space.Self);


        transform.position += transform.forward * Time.deltaTime *10 * booster;
        //print(booster);
        Vector3 camPos = transform.position;
        camPos += -transform.forward * 10f;
        camPos += Vector3.up * 8f;
        cameraObject.transform.position = camPos;

        cameraObject.transform.LookAt(transform.position);

        Timer += Time.deltaTime;

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
