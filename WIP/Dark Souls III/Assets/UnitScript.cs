using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using cakeslice;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;
using Random = UnityEngine.Random;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class UnitScript : MonoBehaviour
{
    public cakeslice.Outline outline;
    public static Action<EnemyScript> Highlight;
    public static Action<EnemyScript> UnHighlight;
    public static UnitScript selectedUnit;
    public GameObject damageTextPrefab;

    public Button ActionSurgeButton;

    public GameObject uifollower;
    public GameObject uifollowtext;
    public GameObject ui;

    public HealthBarScript healthbar;
    public HealthBarScript worldbar;

    public GameObject moveLeft;
    public GameObject apLeft;


    public Camera mainCamera;
    public string unitName;
    public string bio;
    public string stats;

    public NavMeshAgent nma;

    public Renderer bodyRenderer;
    public Color normalColor;
    public Color selectedColor;
    private NavMeshPath path;

    public int maxHealth = 100;
    public int currentHealth;

    //List<EnemyScript> enemyOverwhelm = new List<EnemyScript>();
    //if the player has too many enemies around it, dont push lads (didnt have enough time to do it)
    //public bool weOverwhelmed = false;

    private LineRenderer lineRenderer;

    Collider target;

    public Vector3 destination;

    public bool selected = false;

    public Dictionary<string, bool> options = new Dictionary<string, bool>
    {
        {"move", false},
        {"attack", false}
    };
    //RESET THIS EVERY TURN
    int moveDistance = 15;
    int distanceMoved = 0;

    List<GameObject> attackableFoes = new List<GameObject>();

    int actionPoints = 1;


    float rotateSpeed;

    LayerMask layerMask;

    private GameManager gameManager;



    /*
    TO DO LIST:

    LIMITED TURNS?

    CONTROLS POPUP

    SHOOTING
    ENEMY AI
    */

    void OnEnable()
    {

        if (!selected)
            bodyRenderer.material.color = normalColor;
        GameManager.UnitClicked += GameManagerSaysUnitWasClicked;
        GameManager.playerTurn += playerTurn;
        GameManager.enemyTurn += enemyTurn;
    }

    void OnDisable()
    {
        GameManager.UnitClicked -= GameManagerSaysUnitWasClicked;
        GameManager.playerTurn -= playerTurn;
        GameManager.enemyTurn -= enemyTurn;
    }

    void playerTurn()
    {
        //START OF OUR TURN BABY
        //RESET DISTANCE WE CAN WALK, ACTION POINTS, WHATEVER
        distanceMoved = 0;
        actionPoints = 1;
        //yurp

    }

    void enemyTurn()
    {
        //do all "not selected" actions
        GameManagerSaysUnitWasClicked(null);
    }


    void GameManagerSaysUnitWasClicked(UnitScript unit)
    {
        if (unit == this)
        {
            selected = true;
            bodyRenderer.material.color = selectedColor;
            selectedUnit = this;
            worldbar.transform.parent.gameObject.SetActive(false);

        }
        else
        {
            //delete the line cuz it'll just stay when u switch
            lineRenderer.positionCount = 0;
            selected = false;
            worldbar.transform.parent.gameObject.SetActive(true);
            worldbar.setHealth(currentHealth);
            bodyRenderer.material.color = normalColor;

        }
        gameObject.transform.GetChild(2).gameObject.SetActive(selected);
    }


    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();

        rotateSpeed = Random.Range(20, 60);

        transform.Rotate(0, Random.Range(0, 360), 0);

        layerMask = LayerMask.GetMask("ground", "unit", "enemy");

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;

        currentHealth = maxHealth;
        healthbar.SetMaxHealth(currentHealth);
        worldbar.SetMaxHealth(currentHealth);
        worldbar.transform.parent.gameObject.SetActive(true);
        gameManager = GameManager.instance;


    }





    // Update is called once per frame
    void Update()
    {
        if (!nma.pathPending && nma.remainingDistance <= nma.stoppingDistance)
        {
            nma.ResetPath();
        }


        if (selected)
        {


            //Debug.Log(weOverwhelmed + " " + enemyOverwhelm.Count());


            if (!options["move"])
            {
                //delete the line cuz it'll just stay when u switch
                lineRenderer.positionCount = 0;

                uifollower.gameObject.SetActive(false);
            }



            //if the move button is clicked and we are not currently moving, draw a line showing where the mouse will take us
            if (options["move"] && nma.velocity == Vector3.zero)
            {
                Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask))
                {
                    if (hitInfo.collider.CompareTag("ground"))
                    {
                        NavMesh.CalculatePath(transform.position, hitInfo.point, NavMesh.AllAreas, path);
                        uifollower.gameObject.SetActive(true);
                        DrawPrePath();
                        uifollowtext.GetComponent<TextMeshProUGUI>().text = ((int)GetPathLength(path)).ToString();
                        //Debug.Log("prepath length: " + GetPathLength(path) + " distance traveled: " + distanceMoved);

                        //if we can't move, let the ui element know
                        int pathdist = (int)GetPathLength(path);
                        if (pathdist <= moveDistance - distanceMoved && pathdist != 0)
                        {
                            uifollowtext.GetComponent<TextMeshProUGUI>().color = Color.black;
                        }
                        else
                        {
                            uifollowtext.GetComponent<TextMeshProUGUI>().color = Color.red;
                        }



                    }
                    //if (hitInfo.collider.CompareTag("enemy"))
                    //{
                    //    hitInfo.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = true;
                    //    outline = hitInfo.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<Outline>();
                    //}
                    //if (!hitInfo.collider.CompareTag("enemy"))
                    //{
                    //    Debug.Log("clear");
                    //    if (outline != null)
                    //        outline.enabled = false;
                    //}
                }

            }


            //we have clicked on a thing
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask))
                    {
                        target = hitInfo.collider;

                        performAction(hitInfo);
                    }
                }
            }
            moveLeft.GetComponent<TextMeshProUGUI>().text = $"Move Left: {moveDistance - distanceMoved}/{moveDistance}";
            apLeft.GetComponent<TextMeshProUGUI>().text = $"Action Points: {actionPoints}";
        }

        //if we're moving, draw a path
        if (nma.hasPath)
        {
            DrawPath();
        }






    }

    //here is where you will do everything. ie, if you're in attack mode, it will see what you should do if you click an enemy, something else, etc.
    void performAction(RaycastHit hitInfo)
    {

        if (nma.velocity == Vector3.zero)
        {
            if (hitInfo.collider.CompareTag("ground"))
            {
                //clicked on ground after selecting unit
                if (options["move"])
                {
                    NavMesh.CalculatePath(transform.position, hitInfo.point, NavMesh.AllAreas, path);
                    int pathdist = (int)GetPathLength(path);
                    if (pathdist <= moveDistance - distanceMoved && pathdist != 0)
                    {
                        nma.SetDestination(hitInfo.point);
                        distanceMoved += pathdist;
                    }
                    else
                    {
                        //we couldn't move bc too far
                    }
                }



            }

            if (hitInfo.collider.CompareTag("enemy"))
            {
                if (options["move"])
                {
                    NavMesh.CalculatePath(transform.position, hitInfo.point, NavMesh.AllAreas, path);
                    int pathdist = (int)GetPathLength(path);
                    if (pathdist <= moveDistance - distanceMoved && pathdist != 0)
                    {
                        nma.SetDestination(hitInfo.point);
                        distanceMoved += pathdist;
                    }
                    else
                    {
                        //we couldn't move bc too far
                    }
                }
                //clicked on an enemy after selecting unit
                if (options["attack"])
                {
                    if (actionPoints > 0)
                    {
                        if (attackableFoes.Contains(hitInfo.collider.gameObject))
                        {
                            //if the enemy we clicked on is in range of being attacked
                            Debug.Log("attack!");

                            Attack(hitInfo.collider.gameObject.GetComponent<EnemyScript>());


                            actionPoints--;
                        }
                        else
                        {
                            Debug.Log("cant attack!" + attackableFoes);
                            //the enemey is not in range     
                        }
                    }
                    else
                    {
                        //we don't have enough action points

                    }


                }
            }



        }
    }

    void DrawPrePath()
    {

        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPosition(0, transform.position);

        if (path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 pointPos = path.corners[i];
            lineRenderer.SetPosition(i, pointPos);

        }
    }

    void DrawPath()
    {

        lineRenderer.positionCount = nma.path.corners.Length;
        lineRenderer.SetPosition(0, transform.position);

        if (nma.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < nma.path.corners.Length; i++)
        {
            Vector3 pointPos = nma.path.corners[i];
            lineRenderer.SetPosition(i, pointPos);

        }


    }

    public static float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("enemyRadius"))
        {
            //holy
            Collider parent = other.gameObject.transform.parent.gameObject.GetComponent<Collider>();

            if (target != null && target == parent)
            {

                //we are in range of the enemy we clicked on
                if (nma.hasPath)
                {
                    //when i collide with the enemy radius, stop 
                    nma.ResetPath();
                    //here you would attack
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("enemyRadius"))
        {
            //holy
            Collider parent = other.gameObject.transform.parent.gameObject.GetComponent<Collider>();

            if (target != null && target == parent)
            {

                //we are in range of the enemy we clicked on
                if (nma.hasPath)
                {
                    //when i collide with the enemy radius, stop 
                    nma.ResetPath();
                }
            }
            Debug.Log(parent.gameObject.GetComponent<EnemyScript>());
            if (!attackableFoes.Contains(parent.gameObject))
            {
                attackableFoes.Add(parent.gameObject);
            }
        }
        //if (other.CompareTag("enemy") && !enemyOverwhelm.Contains(other.gameObject.GetComponent<EnemyScript>()))
        //{
        //    Debug.Log("add");
        //    enemyOverwhelm.Append(other.gameObject.GetComponent<EnemyScript>());
        //    if (enemyOverwhelm.Count >= 3)
        //    {
        //        weOverwhelmed = true;
        //    }
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("enemyRadius"))
        {
            Collider parent = other.gameObject.transform.parent.gameObject.GetComponent<Collider>();

            attackableFoes.Remove(parent.gameObject);

            if (nma.hasPath)
            {

                int randomValue = Random.Range(1, 8);
                if (Random.value < 0.4f)
                {
                    TakeDamage(999);
                }
                else
                {
                    TakeDamage(randomValue);
                }
            }

        }
        //if (other.CompareTag("enemy") && enemyOverwhelm.Contains(other.gameObject.GetComponent<EnemyScript>()))
        //{
        //    enemyOverwhelm.Remove(other.gameObject.GetComponent<EnemyScript>());
        //    if (enemyOverwhelm.Count < 3)
        //    {
        //        weOverwhelmed = false;
        //    }
        //}




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
            if (worldbar != null)
            {
                worldbar.setHealth(currentHealth);
            }
        }


        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
        GameObject DamageText = Instantiate(damageTextPrefab, pos, Quaternion.identity);



        DamageText.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = message;

        //if dead, tell game manager
        if (currentHealth <= 0)
        {
            gameManager.EndPlayerLife(this);
            Destroy(gameObject);
        }




    }

    void Attack(EnemyScript enemy)
    {
        int randomValue = Random.Range(20, 40);
        if (Random.value < 0.1f)
        {
            enemy.TakeDamage(999);
        }
        else
        {

            enemy.TakeDamage(randomValue);
        }

    }

    public void ActionSurge()
    {
        actionPoints++;
        ActionSurgeButton.interactable = false;

    }





}
