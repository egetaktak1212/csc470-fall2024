using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static Action SpacebarPressed;

    public static Action friendlyTurn;

    public static Action enemyTurn;

    public static Action<UnitScript> UnitClicked;

    public static GameManager instance;

    public Camera mainCamera;

    public UnitScript selectedUnit;

    public List<UnitScript> units = new List<UnitScript>();



    LayerMask layerMask;

    void OnEnable()
    {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        layerMask = LayerMask.GetMask("ground", "unit");
    }


    void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask))
                {
                    if (hitInfo.collider.CompareTag("unit"))
                    {
                        SelectUnit(hitInfo.collider.gameObject.GetComponent<UnitScript>());
                    }
                }
            }
        }

    }

    public void nextTurn() {
        Debug.Log("guh");
    
    
    }


    public void SelectUnit(UnitScript unit)
    {


        UnitClicked?.Invoke(unit);


        selectedUnit = unit;

    }
}
