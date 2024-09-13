using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class plane_script : MonoBehaviour
{
    float forwardSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vAxis = Input.GetAxis("Vertical");
        float hAxis = Input.GetAxis("Horizontal");

        transform.Rotate(vAxis, 0,hAxis, Space.Self);

        transform.position += transform.forward * forwardSpeed;



    }
}
