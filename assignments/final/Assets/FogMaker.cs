using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMaker : MonoBehaviour
{

    public GameObject prefab;
    public GameObject player;


    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < 10; i++)
        {
            Vector3 theLocation = new Vector3(transform.position.x, transform.position.y + i, transform.position.z);
            GameObject obj = Instantiate(prefab, theLocation, transform.rotation);
            obj.transform.SetParent(transform);

        }
        
        




    }

    // Update is called once per frame
    void Update()
    {
        Vector3 followPlayer = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.position = followPlayer;
    }
}
