using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cakeslice;
using TMPro;
using Unity.VisualScripting;
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
    private GameManager gameManager;

    public Vector3 destination;

    public bool selected = false;

    public Outline moveOut;
    public Outline attackOut;

    float rotateSpeed;

    LayerMask layerMask;

    public int maxHealth = 60;
    public int currentHealth;
    public HealthBarScript healthbar;

    bool retreated = false;

    void OnEnable()
    {

        GameManager.playerTurn += playerTurn;
        GameManager.enemyTurn += enemyTurn;
    }

    void OnDisable()
    {

        GameManager.playerTurn -= playerTurn;
        GameManager.enemyTurn -= enemyTurn;
    }

    void playerTurn()
    {


    }

    void enemyTurn()
    {


        //StartCoroutine(PerformEnemyActions());
    }

    public void StartTurn()
    {

        //ITS OUR TURN BABY NYEH NYEH NYEH
        finishedTurn = false;
        StartCoroutine(PerformEnemyActions());



    }




    //WHERE THE MAGIC HAPPENS BOYS
    private IEnumerator PerformEnemyActions()
    {
        if (lastRetreat) {
            lastRetreat = false;
            finishedTurn = true;
            yield break;
        }

        // If the enemy has less than 30% health, it might retreat
        if (currentHealth < maxHealth * 0.3f)
        {
            float rand = Random.value;
            if (rand < 0.5f && !retreated)
            {
                RetreatFromFoes();
                retreated = true;
                finishedTurn = true;
                yield break;
            }
        }

        // Attempt to approach and attack the nearest unit
        if (true /* I made this only to minimize the below*/)
        {
            UnitScript nearestPlayer = getNearestPlayer();

            if (nearestPlayer != null)
            {
                //Debug.Log("A");
                float distanceToPlayer = Vector3.Distance(transform.position, nearestPlayer.transform.position);
                float attackRange = 2f;
                float moveDistance = 5f; // Set maximum move distance

                if (distanceToPlayer <= attackRange)
                {
                    //Debug.Log("B");
                    // If already in attack range, attack the player
                    Attack(nearestPlayer);
                    finishedTurn = true;
                    yield break; // End turn after attacking
                }
                else
                {

                    if (nma != null)
                    {
                        //Debug.Log("C");
                        nma.isStopped = false;
                        nma.SetDestination(nearestPlayer.transform.position);
                        Vector3 start = transform.position;

                        float distanceMoved = 0f;

                        while (true)
                        {
                            //Debug.Log("D");
                            distanceMoved = Vector3.Distance(start, transform.position);
                            //Debug.Log("E");
                            if (Vector3.Distance(transform.position, nearestPlayer.transform.position) <= attackRange)
                            {
    
                                nma.isStopped = true;
                                Attack(nearestPlayer);
    
                                finishedTurn = true;
                                //Debug.Log("F");
                                yield break;
                            }

                            if (distanceMoved >= moveDistance)
                            {
                                //Debug.Log("G");
                                nma.isStopped = true;  // Stop the enemy from moving.
                                break;                 // Exit the loop as the enemy can't move anymore.
                            }

                            yield return null;
                        }
                    }
                }
            }
        }

        finishedTurn = true;

    }






    private void RetreatFromFoes()
    {
        //if Im low on hp brother, im boutta flee outta here yfeel me. i aint dyin today. But ill only do it once per game cuz i aint a little wuss yfeel
        //this is so sick man
        UnitScript nearestPlayer = getNearestPlayer();

         if (nearestPlayer != null)
        {
            Vector3 retreatDirection = (transform.position - nearestPlayer.transform.position);
            retreatDirection.Normalize();

            float speed = 7f;
            Vector3 retreatPosition = transform.position + retreatDirection * speed;

            if (nma != null)
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);

                GameObject DamageText = Instantiate(damageTextPrefab, pos, Quaternion.identity);
                DamageText.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = "RETREAT!";
                nma.isStopped = false;
                nma.SetDestination(retreatPosition);
                lastRetreat = true;
            }



        }


    }

    UnitScript getNearestPlayer()
    {
        UnitScript[] playerUnits = FindObjectsOfType<UnitScript>();

        UnitScript nearestPlayer = null;
        float closestDistance = Mathf.Infinity;


        foreach (UnitScript player in playerUnits)
        {
            if (player != null && player.gameObject.CompareTag("unit"))
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    nearestPlayer = player;
                }
            }
        }

        return nearestPlayer;

    }




    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        layerMask = LayerMask.GetMask("wall");

        rotateSpeed = Random.Range(20, 60);

        transform.Rotate(0, Random.Range(0, 360), 0);

        layerMask = LayerMask.GetMask("ground", "unit", "enemy");

        moveOut.enabled = false;
        attackOut.enabled = false;
        gameManager = GameManager.instance;


        currentHealth = maxHealth;
        healthbar.SetMaxHealth(currentHealth);


    }

    private void Update()
    {

        handleOutlines();
    }

    void handleOutlines()
    {
        if (UnitScript.selectedUnit != null)
        {
            bool move = UnitScript.selectedUnit.options["move"];
            bool attack = UnitScript.selectedUnit.options["attack"];

            Vector3 selectedUnitPosition = UnitScript.selectedUnit.transform.position;
            float distanceToEnemy = Vector3.Distance(selectedUnitPosition, transform.position);
            //we have to be 2 units from the player for them to be able to attack
            //im in the mind of the goblin writing this
            //player bad
            float attackRange = 2f;

            if (move || attack)
            {
                Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask))
                {
                    if (hitInfo.collider.gameObject == gameObject)
                    {
                        if (move)
                            moveOut.enabled = true;

                        if (attack && distanceToEnemy <= attackRange)
                            attackOut.enabled = true;
                        else
                            attackOut.enabled = false;
                    }
                    else
                    {
                        moveOut.enabled = false;
                        attackOut.enabled = false;
                    }
                }
            }
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


        //if dead, tell game manager
        if (currentHealth <= 0)
        {
            gameManager.EndEnemyLife(this);
            Destroy(gameObject);
        }

        if (currentHealth < maxHealth * 0.3f)
        {
            float rand = Random.value;
            if (rand < 0.5f && !retreated)
            {
                RetreatFromFoes();
                retreated = true;
                finishedTurn = true;
            }
        }



    }

    void Attack(UnitScript player)
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
