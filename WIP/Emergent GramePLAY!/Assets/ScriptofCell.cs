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
    public int gooTimer = 0;
    float simulationTimer;
    float simulationRate = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        SetType();
    }

    // Update is called once per frame
    void Update()
    {
        simulationTimer -= Time.deltaTime;
        if (simulationTimer < 0)
        {
            gooTimer++;
            simulationTimer = simulationRate;
        }
        if (gooTimer == 3) {
            if (type == 3)
            {
                type = 0;
            }
            gooTimer = 0;
        }



    }

    private void OnMouseDown()
    {
        if (xCoord > 35 && xCoord < 40)
        {
            type = 2;
            SetType();
        }
    }


    public void SetType() {
        if (type == 0)
        {
            cellRenderer.material.color = Color.green;
            if (xCoord > 35 && xCoord < 40) {
                cellRenderer.material.color = new Color32(150, 75, 0, 1);
            }
        }
        else if (type == 1)
        {
            cellRenderer.material.color = new Color32(196, 0, 40, 1);
        }
        else if (type == 2)
        {
            cellRenderer.material.color = Color.yellow;
        }
        else if (type == 3) {
            cellRenderer.material.color = new Color32(255, 191, 0, 1);
        }
        else if (type == 4) {
            cellRenderer.material.color = new Color32(164, 0, 82, 1);
        }
        else if (type == 5) {
            cellRenderer.material.color = new Color32(230, 111, 0, 1);
        }




    }

}
