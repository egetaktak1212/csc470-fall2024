using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;

public class EnemyScript : MonoBehaviour
{
    public bool finishedTurn { get; private set; } = false; //i got this one from gpt, google was not enough. i will lament later
    public Camera mainCamera;
    public GameObject damageTextPrefab;
    bool lastRetreat = false;

    public NavMeshAgent nma;

    public Renderer bodyRenderer;

    private NavMeshPath path;
    private GM gameManager;

    public Vector3 destination;

    public bool selected = false;

    float rotateSpeed;

    LayerMask layerMask;

    public int maxHealth = 60;
    public int currentHealth;
    public HealthBarScript healthbar;


    bool retreated = false;

    public bool agro = false;

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }


    PlayerControls getNearestPlayer()
    {
        return FindObjectOfType<PlayerControls>();
    }

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        layerMask = LayerMask.GetMask("wall");

        rotateSpeed = Random.Range(20, 60);

        transform.Rotate(0, Random.Range(0, 360), 0);

        layerMask = LayerMask.GetMask("ground", "unit", "enemy");

        gameManager = GM.instance;

        currentHealth = maxHealth;
        healthbar.setHealth(currentHealth);
        healthbar.SetMaxHealth(currentHealth);
    }

    private void Update()
    {
        Debug.Log($"Agro: {agro}"); // Debug to confirm agro value

        if (agro)
        {
            nma.isStopped = false; // Allow movement
            PlayerControls nearestPlayer = getNearestPlayer();
            if (nearestPlayer != null)
            {
                nma.SetDestination(nearestPlayer.transform.position);
            }
        }
        else
        {
            nma.isStopped = true; // Stop the agent if not in agro mode
        }
    }


    public void TakeDamage(int damage)
    {
        string message;
        if (damage == 999)
        {
            message = "Missed!";
        }
        else
        {
            message = damage.ToString();
            currentHealth -= damage;
            healthbar.setHealth(currentHealth);
        }

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);

        GameObject DamageText = Instantiate(damageTextPrefab, pos, Quaternion.identity);
        DamageText.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = message;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }

    }

    void Attack(PlayerControls player)
    {
        int randomValue = Random.Range(5, 15);

        if (Random.value < 0.4f)
        {
            player.TakeDamage(999);
        }
        else
        {
            player.TakeDamage(randomValue);
        }
    }
}
