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

    private IEnumerator PerformEnemyActions()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("GRAHH I DID MY TURN");

        gameManager.EndEnemiesTurn(this);
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

    void handleOutlines() {
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
