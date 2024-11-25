using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{

    private Camera mainCamera;
    Transform cam;


    private void Start()
    {
        mainCamera = Camera.main;
        cam = mainCamera.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
