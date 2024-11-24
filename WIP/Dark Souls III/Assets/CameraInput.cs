using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    public float moveSpeed = 0.001f;
    public float rotationSpeed = 200f;



    // Update is called once per frame
    void Update()
    {

        float horizontal = Input.GetAxis("Horizontal") * 0.1f;
        float vertical = Input.GetAxis("Vertical") * 0.1f;

        Vector3 movement = (transform.right * horizontal + transform.forward * vertical) * moveSpeed * Time.deltaTime;
        movement.y = 0f;

        transform.Translate(movement, Space.World);  

        float rotation = -Input.GetAxis("EQ");
        transform.Rotate(0f, rotation * rotationSpeed * Time.deltaTime, 0f, Space.World);
    }
}
