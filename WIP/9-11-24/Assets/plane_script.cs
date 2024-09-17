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
    public float forwardSpeed = 90f;
    bool boost = false;
    float Timer;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vAxis = Input.GetAxis("Vertical");
        float hAxis = Input.GetAxis("Horizontal");

        Vector3 amountToRotate = new Vector3(0, 0, 0);
        amountToRotate.x = vAxis * xRotationSpeed;
        amountToRotate.x = vAxis * yRotationSpeed;
        amountToRotate *= Time.deltaTime;
        transform.Rotate(amountToRotate, Space.Self);

        transform.position += transform.forward * forwardSpeed * Time.deltaTime;

        Vector3 camPos = transform.position;
        camPos += -transform.forward * 10f;
        camPos += Vector3.up * 8f;
        cameraObject.transform.position = camPos;

        cameraObject.transform.LookAt(transform.position);

        Timer += Time.deltaTime;

    }

    public void OnCollisionEnter(GameObject other)
    {
        time = Timer;
        if (other.CompareTag("Booster")) {
            Boost();
        }
    }

    public void Boost() {
        forwardSpeed *= 2;
    }


}
