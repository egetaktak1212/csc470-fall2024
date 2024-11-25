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
    public Camera mainCamera;

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

    public int maxHealth = 100;
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
        //ITS OUR TURN BABY NYEH NYEH NYEH

        StartCoroutine(PerformEnemyActions());
    }


    //WHERE THE MAGIC HAPPENS BOYS
    private IEnumerator PerformEnemyActions()
    {
        yield return new WaitForSeconds(2);

        //if we are less than 30 percent health, RUN MAN GET THE HELL OUTTA THERE
        if (currentHealth < maxHealth * 0.3f)
        {
            //but we have sm honor about it, so its a 50% chance
            float rand = Random.value;
            if (rand < 0.5f && !retreated) { 
                RetreatFromFoes();
                //if we retreat once, we dont again
                retreated = true;
                gameManager.EndEnemiesTurn(this);
                yield break;
            }
        }

        //we will attempt to approach and attack the nearest unit
        UnitScript nearestPlayer = getNearestPlayer();

        if (nearestPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, nearestPlayer.transform.position);


            float moveDistance = 5f;  
            float attackRange = 2f;  

            if (distanceToPlayer <= attackRange)
            {
                //attack here

            }
            else
            {
                if (nma != null)
                {
                    nma.SetDestination(nearestPlayer.transform.position);
                    while (Vector3.Distance(transform.position, nearestPlayer.transform.position) > attackRange)
                    {
                        if (Vector3.Distance(transform.position, nearestPlayer.transform.position) <= moveDistance)
                        {
                            nma.isStopped = true;
                            break;
                        }
                        yield return null;
                    }
                }
            }
        }

        Debug.Log("GRAHH I DID MY TURN");



        gameManager.EndEnemiesTurn(this);
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

            if (nma != null) {
                nma.SetDestination(retreatPosition);
                Debug.Log(retreatPosition == transform.position);
            }



        }


    }

    UnitScript getNearestPlayer() {
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.setHealth(currentHealth);

        //if dead, tell game manager
        if (currentHealth <= 0)
        {

            gameManager.EndEnemyLife(this);
            Destroy(gameObject);
        }


    }






}
