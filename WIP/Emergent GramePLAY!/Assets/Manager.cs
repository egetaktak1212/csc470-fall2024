using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject prefab;

    ScriptofCell[,] grid;
    float spacing = 1.1f;

    float simulationTimer;
    float simulationRate = 0.5f;
    int bombtimer = 0;
    int tester = 0;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        simulationTimer = simulationRate;
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
                grid[x, y].type = 0;
                grid[x, y].xCoord = x;
                grid[x, y].yCoord = y;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        System.Random rng = new System.Random();
        simulationTimer -= Time.deltaTime;
        if (simulationTimer < 0)
        {
            if (bombtimer == 2) {
                int rando = rng.Next(0, 10);
                grid[0, rando].type = 1;
                grid[0, rando].SetType();
                
                bombtimer = 0;
            }
            tester++;
            Simulate();
            
            simulationTimer = simulationRate;
            bombtimer++;
        }
        int blackCount = 0;
        for (int y = 0; y < 10; y++) {
            if (grid[9, y].type == 3)
            {
                blackCount++;
            }
        }
        if (blackCount > 4) {
            Debug.Log("gameEnd");
            Time.timeScale = 0;
        }
        
    }

    void Simulate()
    {
        
        

        int[,] nextType = new int[10, 10];
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                nextType[x, y] = grid[x, y].type;
            }
        }


        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (x == 0 && grid[x, y].type == 2)
                {
                    nextType[x, y] = 0;
                }
                else if (x < 8 && grid[x, y].type == 1 && grid[x + 2, y].type == 2) {
                    nextType[x, y] = 0;
                    nextType[x + 2, y] = 0;
                    grid[x + 2, y].type = 0;
                }
                //bomb hits black, bomb dissappears
                else if (x != 9 && grid[x, y].type == 1 && grid[x + 1, y].type == 3) {
                    nextType[x, y] = 0;
                }
                //bomb moves forward as long as its not arrow ahead
                else if (grid[x, y].type == 1 && x != 9 && grid[x + 1, y].type != 2)
                {
                    nextType[x + 1, y] = 1;
                    nextType[x, y] = 0;
                }
                else if (grid[x, y].type == 2 && x != 0 && grid[x - 1, y].type != 1)
                {
                    nextType[x - 1, y] = 2;
                    nextType[x, y] = 0;
                }
                //bomb and arrow dissapears if they collide
                else if (grid[x, y].type == 1 && x != 9 && grid[x + 1, y].type == 2)
                {
                    nextType[x, y] = 0;
                    nextType[x + 1, y] = 0;

                }
                else if (x == 9 && grid[x, y].type == 1)
                {
                    nextType[x, y] = 3;
                }
            }
        }
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                grid[x, y].type = nextType[x, y];
                grid[x, y].SetType();

            }
        }
        


    }


}
