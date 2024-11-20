using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static Action SpacebarPressed;

    public static Action<UnitScript> UnitClicked;

    public static GameManager instance;

    public Camera mainCamera;

    public UnitScript selectedUnit;

    public List<UnitScript> units = new List<UnitScript>();

    public GameObject popUpWindow;

    public TMP_Text nameText;
    public TMP_Text bioText;
    public TMP_Text statText;
    public Image portraitImage;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {

            SpacebarPressed?.Invoke();
        }


        if (Input.GetMouseButtonDown(0))
        {
            Ray mousePositionRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mousePositionRay, out hitInfo, Mathf.Infinity, layerMask))
            {
                if (hitInfo.collider.CompareTag("ground"))
                {
                    Debug.Log("buh");
                    if (selectedUnit != null)
                    {
                        selectedUnit.nma.SetDestination(hitInfo.point);
                        
                    }
                }
                else if (hitInfo.collider.CompareTag("unit"))
                {
                    SelectUnit(hitInfo.collider.gameObject.GetComponent<UnitScript>());
                }
            }
        }

    }

    //public void OpenCharacterSheet()
    //{
    //    if (selectedUnit == null) return;

    //    nameText.text = selectedUnit.unitName;
    //    bioText.text = selectedUnit.bio;
    //    statText.text = selectedUnit.stats;

    //    popUpWindow.SetActive(true);
    //}

    public void SelectUnit(UnitScript unit)
    {


        UnitClicked?.Invoke(unit);


        selectedUnit = unit;

    }

    //public void ClosePopUpWindow()
    //{
    //    popUpWindow.SetActive(false);
    //}
}