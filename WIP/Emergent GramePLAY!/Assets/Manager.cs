using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class Manager : MonoBehaviour
{
    public GameObject prefab;
    public GameObject youwin;
    public GameObject youlose;


    ScriptofCell[,] grid;
    ScriptofCell[,] life;
    float spacing = 1.1f;

    float simulationTimer;
    float simulationRate = 0.5f;
    float simulationTimer2;
    float simulationRate2 = 0.25f;
    int bombtimer = 0;
    int tester = 0;
    System.Random rng = new System.Random();
    // Start is called before the first frame update
    void Start()
    {
        youwin.SetActive(false);
        youlose.SetActive(false);
        Time.timeScale = 1;
        simulationTimer = simulationRate;
        grid = new ScriptofCell[40, 10];
        life = new ScriptofCell[6,10];
        for (int x = 0; x < 46; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector3 pos = transform.position;
                pos.x += x * spacing;
                pos.z += y * spacing;
                GameObject cell = Instantiate(prefab, pos, Quaternion.identity);
                if (x > 39)
                {
                    life[x - 40, y] = cell.GetComponent<ScriptofCell>();
                    life[x - 40, y].type = rng.Next(4, 6);
                    life[x - 40, y].xCoord = x;
                    life[x - 40, y].yCoord = y;
                }
                else
                {
                    grid[x, y] = cell.GetComponent<ScriptofCell>();
                    grid[x, y].type = 0;
                    grid[x, y].xCoord = x;
                    grid[x, y].yCoord = y;
                }
            }
        }
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                grid[x, y].type = rng.Next(0, 2);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (winCheck()) {
            youwin.SetActive(true);
            Time.timeScale = 0;
        }


        simulationTimer -= Time.deltaTime;
        if (simulationTimer < 0)
        {
            Simulate();
            OtherSimulate();
            lifeKills();

            simulationTimer = simulationRate;

        }
        




        for (int y = 0; y < 10; y++)
        {
            if (grid[35, y].type == 1)
            {
                youlose.SetActive(true);
                Time.timeScale = 0;
            }
        }

    }

    void Simulate()
    {
        
        

        int[,] nextType = new int[40, 10];
        for (int x = 0; x < 40; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                nextType[x, y] = grid[x, y].type;
            }
        }


        for (int x = 0; x < 40; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (x == 0 && grid[x, y].type == 2)
                {
                    nextType[x, y] = 0;
                }
                else if (x < 38 && grid[x, y].type == 1 && grid[x + 2, y].type == 2)
                {
                    GooTurner(x, y, nextType);
                    nextType[x, y] = 0;
                    nextType[x + 2, y] = 0;
                    grid[x + 2, y].type = 0;
                }
                else if (grid[x, y].type == 0 && x < 36 && CountRed(x, y) >= 2 && CountGoo(x, y, nextType) == 0)
                {
                    nextType[x, y] = 1;
                }
                else if (grid[x, y].type == 1 && x < 36 && CountGoo(x, y, nextType) > 2)
                {
                    nextType[x, y] = 0;
                }
                else if (grid[x, y].type == 2 && x != 0 && grid[x - 1, y].type != 1 && grid[x - 1, y].type != 3)
                {
                    nextType[x - 1, y] = 2;
                    nextType[x, y] = 0;
                }
                //bomb and arrow dissapears if they collide
                else if (grid[x, y].type == 1 && x != 39 && grid[x + 1, y].type == 2)
                {
                    GooTurner(x, y, nextType);
                    nextType[x + 1, y] = 0;
                    nextType[x, y] = 3;

                }
                else if (grid[x, y].type == 3 && CountRed(x, y) != 0)
                {
                    GooTurner(x, y, nextType);
                    nextType[x + 1, y] = 0;
                    nextType[x, y] = 3;

                }
                else if (grid[x, y].type == 2 && x != 0 && grid[x - 1, y].type == 3)
                {
                    nextType[x, y] = 3;
                }
                else if (grid[x, y].type == 2 && x > 1 && grid[x - 2, y].type == 3)
                {
                    nextType[x,y] = 3;
                }



            }
        }
        for (int x = 0; x < 40; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                grid[x, y].type = nextType[x, y];
                grid[x, y].SetType();

            }
        }
        

    }

    public int CountRed(int xIndex, int yIndex)
    {
        int count = 0;

        for (int x = xIndex - 1; x <= xIndex + 1; x++)
        {
            for (int y = yIndex - 1; y <= yIndex + 1; y++)
            {
                if (x >= 0 && x < 36 && y >= 0 && y < 10)
                {
                    // This if makes sure we don't consider the cell itself while counting its
                    // neighbors
                    if (!(x == xIndex && y == yIndex))
                    {
                        // If one of the surrounding cells is alive, increment the alive count.
                        if (grid[x, y].type == 1)
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }
    public int CountGoo(int xIndex, int yIndex, int[,] next)
    {
        int count = 0;

        for (int x = xIndex - 1; x <= xIndex + 1; x++)
        {
            for (int y = yIndex - 1; y <= yIndex + 1; y++)
            {
                if (x >= 0 && x < 36 && y >= 0 && y < 10)
                {
                    // This if makes sure we don't consider the cell itself while counting its
                    // neighbors
                    if (!(x == xIndex && y == yIndex))
                    {
                        // If one of the surrounding cells is alive, increment the alive count.
                        if (next[x, y] == 3)
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }
    public void GooTurner(int xIndex, int yIndex, int[,] next)
    {
        Debug.Log("gooed up");
        int count = 0;

        for (int x = xIndex - 1; x <= xIndex + 1; x++)
        {
            for (int y = yIndex - 1; y <= yIndex + 1; y++)
            {
                if (x >= 0 && x < 36 && y >= 0 && y < 10)
                {
                    // This if makes sure we don't consider the cell itself while counting its
                    // neighbors
                    if (!(x == xIndex && y == yIndex))
                    {

                        next[x, y] = 3;
                       
                    }
                }
            }
        }


    }

    public bool winCheck() {
        for (int x = 0; x < 40; x++) {
            for (int y = 0; y < 10; y++) {
                if (grid[x, y].type == 1) {
                    return false;
                }
            }
        }
        return true;
    }

    public int CountNeighbors(int xIndex, int yIndex)
    {
        int count = 0;

        for (int x = xIndex - 1; x <= xIndex + 1; x++)
        {
            for (int y = yIndex - 1; y <= yIndex + 1; y++)
            {
                if (x >= 0 && x < 6 && y >= 0 && y < 10)
                {
                    if (!(x == xIndex && y == yIndex))
                    {
                        if (life[x, y].type == 4)
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }

    void OtherSimulate()
    {
        int[,] nextAlive = new int[6, 10];
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                int neighborCount = CountNeighbors(x, y);
                if (life[x, y].type == 4 && neighborCount < 2)
                {
                    // underpopulation
                    nextAlive[x, y] = 5;
                }
                else if (life[x, y].type == 4 && (neighborCount == 2 || neighborCount == 3))
                {
                    // healthy community
                    nextAlive[x, y] = 4;
                }
                else if (life[x, y].type == 4 && neighborCount > 3)
                {
                    // overpopulation
                    nextAlive[x, y] = 5;
                }
                else if (life[x, y].type != 4 && neighborCount == 3)
                {
                    // reproduction
                    nextAlive[x, y] = 4;
                }
                else
                {
                    nextAlive[x, y] = life[x, y].type;
                }
            }
        }
        bool same = true;
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (life[x, y].type != nextAlive[x, y]) {
                    same = false;
                }
            }
        }

                


        // Copy over updated values
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                // Copy over the updated value

                if (!same)
                {
                    life[x, y].type = nextAlive[x, y];
                }
                else {
                    life[x, y].type = rng.Next(4, 6);
                }

                life[x, y].SetType();
            }
        }
    }
    void lifeKills() {
        for (int y = 0; y < 10; y++)
        {
            if (life[0, y].type == 4) { 
                grid[39, y].type = 2;
            } 
        }
    
    }



}
