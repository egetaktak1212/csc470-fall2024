using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class rowmaker : MonoBehaviour
{
    public GameObject sphere;
    GameObject[] array = new GameObject[200];
    // Start is called before the first frame update
    void Start()
    {
        float height = 0;
        float mult = 10;
        int index = 0;
        for (float i = 0; i < 6 * Mathf.PI; i += Mathf.PI / 12)
        {
            GameObject cell = Instantiate(sphere, (new Vector3(mult * Mathf.Cos(i), height, mult * Mathf.Sin(i))), Quaternion.identity);
            array[index] = cell;
            index++;
            mult -= 0.1f;
            height += 0.5f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
