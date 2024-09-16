using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float speed = 0.09f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.position += transform.up * speed;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position += -transform.up * speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += -transform.right * speed;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.position += transform.right * speed;
        }



    }

}