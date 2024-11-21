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


public class UnitScript : MonoBehaviour
{
    public Outline outline;
    public static Action<EnemyScript> Highlight;
    public static Action<EnemyScript> UnHighlight;

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

    Dictionary<string, bool> options = new Dictionary<string, bool>
    {
        {"move", true}, 
        {"shoot", false}
    };


    float rotateSpeed;

    LayerMask layerMask;

    void OnEnable()
    {
        if (!selected)
            bodyRenderer.material.color = normalColor;
        GameManager.SpacebarPressed += ChangeToRandomColor;
        GameManager.UnitClicked += GameManagerSaysUnitWasClicked;
    }

    void OnDisable()
    {
        GameManager.SpacebarPressed -= ChangeToRandomColor;
        GameManager.UnitClicked -= GameManagerSaysUnitWasClicked;
    }




    void ChangeToRandomColor()
    {
        bodyRenderer.material.color = new Color(Random.value, Random.value, Random.value);
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
            selected = false;
            //delete the line cuz it'll just stay when u switch
            lineRenderer.positionCount = 0;
            bodyRenderer.material.color = normalColor;
        }
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





            //if the move button is clicked and we are not currently moving, draw a line showing where the mouse will take us
            if (options["move"] && nma.velocity == Vector3.zero)
            {
                Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask))
                {
                    if (hitInfo.collider.CompareTag("ground")) {
                        NavMesh.CalculatePath(transform.position, hitInfo.point, NavMesh.AllAreas, path);
                        DrawPrePath();
                        
                    }
                    if (hitInfo.collider.CompareTag("enemy")) {
                        hitInfo.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = true;
                        outline = hitInfo.collider.gameObject.transform.GetChild(0).gameObject.GetComponent<Outline>();
                    }
                    if (!hitInfo.collider.CompareTag("enemy") && (target == null || target != hitInfo.collider)) {
                        Debug.Log("target != hitInfo.collider: " + (target != hitInfo.collider));
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

        Debug.Log(target);
        


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
                    nma.SetDestination(hitInfo.point);

                }



            }

            if (hitInfo.collider.CompareTag("enemy"))
            {

                //clicked on an enemy after selecting unit
                if (!options["shoot"]) {
                    nma.SetDestination(hitInfo.point);
                    
                
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
