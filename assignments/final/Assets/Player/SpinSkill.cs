using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSkill : MonoBehaviour
{
    public bool selected = false; // Whether the skill is selected
    public bool spinActive = false; // Whether the skill is active (triggered by first click)
    public int attack = 8; // Damage amount
    public int stamina = 40;
    public float attackInterval = 0.5f; // Interval between attacks
    private float skillTime = 0.6f; // Duration for which the skill is active
    private float timer = 0f; // Timer for skill duration

    public GameObject spinCircle;

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>(); // Track active coroutines

    private void Start()
    {
        spinCircle.SetActive(false);
    }


    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    selected = true;
        //}

        // Activate skill
        if (selected && Input.GetMouseButtonDown(0) && !spinActive)
        {
            // Deduct stamina from GM
            if (GM.instance.stamina >= stamina)
            {
                GM.instance.publicStamina = stamina;
                GM.instance.recoverS = false;
                spinActive = true;
                timer = skillTime;
                StartCoroutine(SkillDuration());
            }
            else
            {
                Debug.Log("Not enough stamina to activate skill!");
            }
        }

        //Debug.Log(timer);
        spinCircle.SetActive(spinActive);


    }

    private IEnumerator SkillDuration()
    {
        Debug.Log("Skill activated!");
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("Skill deactivated!");
        spinActive = false;
        ResetSkill();
    }

    private void ResetSkill()
    {
        timer = 0f;
        StopAllActiveCoroutines();
        Debug.Log("Skill variables reset.");
    }

    private void StopAllActiveCoroutines()
    {
        foreach (var coroutine in activeCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!spinActive)
        {
            //Debug.Log("Skill not active; no damage will be applied.");
            return;
        }

        //Debug.Log($"Trigger detected with: {other.gameObject.name}");
        if (other.CompareTag("unit"))
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                //Debug.Log($"Enemy found: {other.gameObject.name}");
                if (!activeCoroutines.ContainsKey(other.gameObject))
                {
                    //Debug.Log($"Starting ApplyDamage coroutine for: {other.gameObject.name}");
                    Coroutine damageCoroutine = StartCoroutine(ApplyDamage(enemy, other.gameObject));
                    activeCoroutines.Add(other.gameObject, damageCoroutine);
                }
                else
                {
                    Debug.Log($"ApplyDamage already running for: {other.gameObject.name}");
                }
            }
        }
    }

    private IEnumerator ApplyDamage(EnemyScript enemy, GameObject obj)
    {
        while (spinActive)
        {
            Debug.Log($"Applying damage to: {enemy.gameObject.name}");
            enemy.TakeDamage(attack);
            yield return new WaitForSeconds(attackInterval);
        }

        Debug.Log($"Stopping ApplyDamage for: {enemy.gameObject.name}");
        activeCoroutines.Remove(obj);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger exited with: {other.gameObject.name}");
        if (activeCoroutines.ContainsKey(other.gameObject))
        {
            StopCoroutine(activeCoroutines[other.gameObject]);
            activeCoroutines.Remove(other.gameObject);
        }
    }
}
