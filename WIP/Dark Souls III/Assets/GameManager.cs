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

    public GameObject enemyTurnBox;
    public GameObject playerTurnBox;
    Color themsTurn = new Color(0.882f, 1, 0.847f, 1);

    //false is enemy turn, true is player turn
    bool turn = true;


    bool enemiesTurnInProgress = false;


    private List<EnemyScript> activeEnemies = new List<EnemyScript>();

    private List<EnemyScript> aliveEnemies = new List<EnemyScript>();


    private List<UnitScript> activePlayers = new List<UnitScript>();

    public GameObject nextTurnButton;

    public GameObject youWin;
    public GameObject youLose;



    //private int currentEnemy = 0;

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

        playerTurnBox.GetComponent<Image>().color = themsTurn;

        //makes a list of units, so we can check if people are dying man
        activePlayers.AddRange(FindObjectsOfType<UnitScript>());
        aliveEnemies.AddRange(FindObjectsOfType<EnemyScript>());

        youWin.gameObject.SetActive(false);
        youLose.gameObject.SetActive(false);

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
    public void nextTurn()
    {
        //if we are allowed to go to the next turn, flip who's turn it is, and evoke the appropriate stuffs
        if (!enemiesTurnInProgress)
        {
            turn = !turn;

            if (turn)
            {
                playerTurnBox.GetComponent<Image>().color = themsTurn;
                enemyTurnBox.GetComponent<Image>().color = Color.white;
                playerTurn?.Invoke();
            }
            else
            {
                playerTurnBox.GetComponent<Image>().color = Color.white;
                enemyTurnBox.GetComponent<Image>().color = themsTurn;
                enemiesTurnInProgress = true;
                StartEnemyTurn();
                enemyTurn?.Invoke();
            }
        }
    }

    public void EndEnemiesTurn()
    {

        enemiesTurnInProgress = false;
        nextTurnButton.SetActive(true);
        activeEnemies.Clear();
        nextTurn();

    }

    public void EndPlayerLife(UnitScript player)
    {
        Debug.Log("were here");
        if (activePlayers.Contains(player))
        {
            activePlayers.Remove(player);
        }
        if (activePlayers.Count == 0)
        {
            //all players are dead
            Debug.Log("GAME OVER!");
            GameOver(false);


        }
    }
    public void EndEnemyLife(EnemyScript enemy)
    {
        if (aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
        }
        if (aliveEnemies.Count == 0)
        {
            Debug.Log("WE WIN!!");
            GameOver(true);
        }
    }

    public void StartEnemyTurn()
    {
        nextTurnButton.SetActive(false);
        activeEnemies.AddRange(FindObjectsOfType<EnemyScript>());

        if (activeEnemies.Count > 0)
        {
            DoEnemyTurn();
        }
    }

    private void DoEnemyTurn()
    {
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            EnemyScript currentEnemy = activeEnemies[i];
            currentEnemy.StartTurn();
        }
        EndEnemiesTurn();

    }




    public void SelectUnit(UnitScript unit)
    {


        UnitClicked?.Invoke(unit);


        selectedUnit = unit;

    }

    void GameOver(bool win) {
        if (win)
        {

            youWin.gameObject.SetActive(true);
            Time.timeScale = 0f;

        }
        else if (!win) {

            youLose.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }         
    
    }


}
