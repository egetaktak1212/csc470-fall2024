using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{

    public Transform toMove;
    TextMeshPro text;
    public bool canWeMove = false;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
    }





    // Update is called once per frame
    void Update()
    {
        toMove.position = Mouse.current.position.ReadValue() + new Vector2(30,10);
        
        

        

    }
}
