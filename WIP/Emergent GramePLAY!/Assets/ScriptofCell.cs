using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScriptofCell : MonoBehaviour
{

    public Renderer cellRenderer;
    //0 is land, 1 is infantry, 2 is smth else, so 
    public int type = 0;
    public int xCoord;
    public int yCoord;

    // Start is called before the first frame update
    void Start()
    {
        SetType();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("pressed");
        //    type++;
        //    if (type > 2)
        //    {
        //        type = 0;
        //    }
        //    SetType();
        //}
    }

    private void OnMouseDown()
    {
        Debug.Log("pressed");
        type++;
        if (type > 2)
        {
            type = 0;
        }
        SetType();
    }


    public void SetType() {
        if (type == 0)
        {
            cellRenderer.material.color = Color.green;
        }
        else if (type == 1) {
            cellRenderer.material.color = Color.magenta;
        }
        else if (type == 2)
        {
            cellRenderer.material.color = Color.yellow;
        }


    }

}
