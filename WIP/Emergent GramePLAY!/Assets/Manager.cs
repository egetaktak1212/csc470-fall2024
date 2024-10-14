using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject prefab;

    ScriptofCell[,] grid;
    float spacing = 1.1f;



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("herehere");
        grid = new ScriptofCell[10, 10];
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector3 pos = transform.position;
                pos.x += x * spacing;
                pos.z += y * spacing;
                GameObject cell = Instantiate(prefab, pos, Quaternion.identity);
                grid[x, y] = cell.GetComponent<ScriptofCell>();
                grid[x, y].type = 0; // Assign random true or false to the alive of the cell.
                grid[x, y].xCoord = x;
                grid[x, y].yCoord = y;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
