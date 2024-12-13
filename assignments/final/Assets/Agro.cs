using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agro : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Get the EnemyScript component from the parent GameObject
            EnemyScript enemyScript = transform.parent.GetComponent<EnemyScript>();

            if (enemyScript != null)
            {
                // Set the agro bool to true
                enemyScript.agro = true;
                gameObject.GetComponent<Collider>().enabled = false;
                Debug.Log("Player detected! Enemy is now in agro mode.");
            }
        }
    }
}