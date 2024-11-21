using System.Collections;
using System.Collections.Generic;
using System.IO;
using cakeslice;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;


public class EnemyScript : MonoBehaviour
{
    public Camera mainCamera;
    public string unitName;
    public string bio;
    public string stats;

    public Outline outline;

    public NavMeshAgent nma;

    public Renderer bodyRenderer;
    public Color normalColor;
    public Color selectedColor;
    private NavMeshPath path;

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
    }

    void OnDisable()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        layerMask = LayerMask.GetMask("wall");

        rotateSpeed = Random.Range(20, 60);

        transform.Rotate(0, Random.Range(0, 360), 0);

        layerMask = LayerMask.GetMask("ground", "unit");
        
        gameObject.transform.GetChild(0).gameObject.gameObject.GetComponent<Outline>().enabled = false;



    }

    void OnDestroy()
    {
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
                    NavMesh.CalculatePath(transform.position, hitInfo.point, NavMesh.AllAreas, path);
                    //DrawPrePath();
                }

            }



            if (Input.GetMouseButtonDown(0))
            {            
                Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask))
                {
                    Debug.Log("HERE");
                    performAction(hitInfo);
                }
            }



        }
    }

    //here is where you will do everything. ie, if you're in attack mode, it will see what you should do if you click an enemy, something else, etc.
    void performAction(RaycastHit hitInfo) {
        
        if (nma.velocity == Vector3.zero)
        {
            if (hitInfo.collider.CompareTag("ground"))
            {
                if (options["move"])
                    nma.SetDestination(hitInfo.point);


            }
        }
    }
    void highlightEnemy(EnemyScript enemy)
    {
        
        outline = gameObject.transform.GetChild(0).gameObject.GetComponent<Outline>();
        Debug.Log(outline);
        if (enemy == this)
        {
            outline.enabled = true;

        }
        else
        {
            outline.enabled = false;
        }
    }
    void unHighlightEnemy(EnemyScript enemy)
    {

        outline = gameObject.transform.GetChild(0).gameObject.GetComponent<Outline>();
        Debug.Log("un" + outline);
        if (enemy == this)
        {
            outline.enabled = false;
        }

    }




}
