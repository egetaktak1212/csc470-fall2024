using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using cakeslice;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;
using Random = UnityEngine.Random;
using TMPro;

public class UnitScript : MonoBehaviour
{
    public Outline outline;
    public static Action<EnemyScript> Highlight;
    public static Action<EnemyScript> UnHighlight;

    public GameObject uifollower;
    public GameObject uifollowtext;
    public GameObject ui;

    public Camera mainCamera;
    public string unitName;
    public string bio;
    public string stats;

    public NavMeshAgent nma;

    public Renderer bodyRenderer;
    public Color normalColor;
    public Color selectedColor;
    private NavMeshPath path;

    private LineRenderer lineRenderer;

    Collider target;

    public Vector3 destination;

    public bool selected = false;

    public Dictionary<string, bool> options = new Dictionary<string, bool>
    {
        {"move", false}, 
        {"shoot", false}
    };
    //RESET THIS EVERY TURN
    int moveDistance = 15;
    int distanceMoved = 0;


    float rotateSpeed;

    LayerMask layerMask;


    /*
    TO DO LIST:

    NEW BUG:
    When enemy is selected (mouse hovered over it), the outline appears but doesnt go away :)

    RESET ACTION POINTS EVERY TURN 50%
    CANNOT SKIP TURN DURING ENEMY TURN
    LIMITED TURNS?
    TURN HUD AT THE TOP SHOWING WHO"S TURN IT BE

    CONTROLS POPUP

    HEALTHBARS
    SHOOTING
    ATTACKING
    ENEMY AI

    GAMEMANAGER LINE 82, "CANWE", ARE WE ALLOWED TO GO NEXT TURN RN?


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

    void playerTurn() { 
    //START OF OUR TURN BABY
    //RESET DISTANCE WE CAN WALK, ACTION POINTS, WHATEVER
        distanceMoved = 0;
        //actionPoints = 0;
        //yurp

    }

    void enemyTurn() {
        //do all "not selected" actions
        GameManagerSaysUnitWasClicked(null);
    }


    void GameManagerSaysUnitWasClicked(UnitScript unit)
    {
        if (unit == this)
        {
            selected = true;
            bodyRenderer.material.color = selectedColor;

        }
        else
        {
            //delete the line cuz it'll just stay when u switch
            lineRenderer.positionCount = 0;
            selected = false;
            
            bodyRenderer.material.color = normalColor;

        }
        gameObject.transform.GetChild(2).gameObject.SetActive(selected);
    }


    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        layerMask = LayerMask.GetMask("wall");
        rotateSpeed = Random.Range(20, 60);

        transform.Rotate(0, Random.Range(0, 360), 0);

        layerMask = LayerMask.GetMask("ground", "unit", "enemy");

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 0;




    }

    void OnDestroy()
    {
        GameManager.instance.units.Remove(this);
    }



    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
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
                    if (hitInfo.collider.CompareTag("ground")) {
                        NavMesh.CalculatePath(transform.position, hitInfo.point, NavMesh.AllAreas, path);
                        uifollower.gameObject.SetActive(true);
                        DrawPrePath();
                        uifollowtext.GetComponent<TextMeshProUGUI>().text = ((int) GetPathLength(path)).ToString();
                        //Debug.Log("prepath length: " + GetPathLength(path) + " distance traveled: " + distanceMoved);

                        //if we can't move, let the ui element know
                        int pathdist = (int)GetPathLength(path);
                        if (pathdist <= moveDistance - distanceMoved && pathdist != 0)
                        {
                            uifollowtext.GetComponent<TextMeshProUGUI>().color = Color.black;
                        } else {
                            uifollowtext.GetComponent<TextMeshProUGUI>().color = Color.red;
                        }



                    }
                    if (hitInfo.collider.CompareTag("enemy")) {
                        hitInfo.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = true;
                        outline = hitInfo.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<Outline>();
                    }
                    if (!hitInfo.collider.CompareTag("enemy") && (target == null || target != hitInfo.collider)) {
                        if (outline != null)
                            outline.enabled = false;
                    }
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
        }

        //if we're moving, draw a path
        if (nma.hasPath)
        {
            DrawPath();
        }

        Debug.Log(moveDistance - distanceMoved);
        


    }

    //here is where you will do everything. ie, if you're in attack mode, it will see what you should do if you click an enemy, something else, etc.
    void performAction(RaycastHit hitInfo) {
        
        if (nma.velocity == Vector3.zero)
        {
            if (hitInfo.collider.CompareTag("ground"))
            {
                //clicked on ground after selecting unit
                if (options["move"])
                {
                    NavMesh.CalculatePath(transform.position, hitInfo.point, NavMesh.AllAreas, path);
                    int pathdist = (int) GetPathLength(path);
                    if (pathdist <= moveDistance - distanceMoved && pathdist != 0)
                    {
                        nma.SetDestination(hitInfo.point);
                        distanceMoved += pathdist;
                    }
                    else { 
                    //we couldn't move bc too far
                    }
                }



            }

            if (hitInfo.collider.CompareTag("enemy"))
            {

                //clicked on an enemy after selecting unit
                if (options["attack"]) {
                    //attack
                    
                
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
        

        if (other.CompareTag("enemyRadius")) {
            //holy
            Collider parent = other.gameObject.transform.parent.gameObject.GetComponent<Collider>();
            
            if (target != null && target == parent) {
                
                //we are in range of the enemy we clicked on
                if (nma.hasPath) {
                    //when i collide with the enemy radius, stop 
                    nma.ResetPath();
                    //here you would attack
                }
            }
        }
    }







}
