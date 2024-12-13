using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SlashSkill : MonoBehaviour
{
    public bool selected = false; // Whether the skill is selected
    public int attack = 15; // Damage amount
    public int stamina = 10; // Stamina cost (for potential use)
    private bool canDealDamage = false; // Whether the skill is ready to deal damage
    private float cooldown = 0f;
    public float cooldownTime = .5f;
    bool cooldownBoolean = true;

    private Coroutine attackCoroutine; // Coroutine to manage periodic attacks

    private void Update()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        }
        else
        {
            cooldown = 0f;
        }

        // Check if the skill is selected and the left mouse button is clicked
        if (selected && Input.GetMouseButtonDown(0) && cooldown == 0f) // 0 = left mouse button
        {
            canDealDamage = true; // Enable damage for this frame
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("unit"))
        {
            // Player skill attack logic
            if (canDealDamage)
            {
                EnemyScript enemy = other.GetComponent<EnemyScript>();
                if (enemy != null && GM.instance.stamina > stamina)
                {
                    // Deal damage
                    enemy.TakeDamage(attack);
                    cooldown = cooldownTime;
                    GM.instance.publicStamina = stamina;
                    GM.instance.recoverS = false;
                }
                canDealDamage = false; // Reset damage ability after dealing damage
            }
        }

        if (other.CompareTag("Player") && this.CompareTag("unit"))
        {
            EnemyScript enemy = this.GetComponent<EnemyScript>();

            if (enemy.agro && cooldownBoolean) { 
                other.GetComponent<PlayerControls>().TakeDamage(attack-10);
            }



            // Start the enemy attack coroutine when a player enters the range
            //if (attackCoroutine == null)
            //{
            //    attackCoroutine = StartCoroutine(EnemyAttackRoutine(other));
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && this.CompareTag("unit"))
        {
            Debug.Log("coroutine null: " + attackCoroutine != null);
            // Stop the enemy attack coroutine when the player leaves the range
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator WaitCooldown() { 
        cooldownBoolean = false;
        yield return new WaitForSeconds(1f);
        cooldownBoolean = true;
    
    }



    private IEnumerator EnemyAttackRoutine(Collider player)
    {
        PlayerControls playerControls = player.GetComponent<PlayerControls>();

        while (true)
        {
            Debug.Log("attacking");
            // Check if player is still valid and alive
            if (player != null && player.CompareTag("Player") && playerControls.currentHealth > 0)
            {
                playerControls.TakeDamage(attack - 5);
                cooldown = 1f;
                yield return new WaitForSeconds(1f); // Wait for 1 second between attacks
            }
            else
            {
                yield break; // Exit coroutine if player is no longer valid or alive
            }
        }
    }
}