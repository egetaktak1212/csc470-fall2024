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

    public static Action enemyTurn;

    public static Action playerTurn;

    public static Action<UnitScript> UnitClicked;

    public static Action deselectAll;

    public static GameManager instance;

    public Camera mainCamera;

    public UnitScript selectedUnit;

    public List<UnitScript> units = new List<UnitScript>();

    //false is enemy turn, true is player turn

    bool turn = true;





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
                    if (hitInfo.collider.CompareTag("unit") && turn/*if its player turn */)
                    {
                        SelectUnit(hitInfo.collider.gameObject.GetComponent<UnitScript>());
                    }
                }
            }
        }

    }

    //when next turn is pressed, this goes off
    public void nextTurn() {
        //FIX THIS
        bool canWe = true;

        //if we are allowed to go to the next turn, flip who's turn it is, and evoke the appropriate stuffs
        if (canWe) {
            turn = !turn;
            if (turn)
            {
                playerTurn?.Invoke();
            }
            else { 
                enemyTurn?.Invoke();
            }

        }

    
    
    }


    public void SelectUnit(UnitScript unit)
    {


        UnitClicked?.Invoke(unit);


        selectedUnit = unit;

    }
}
