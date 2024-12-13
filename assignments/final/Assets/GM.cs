using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GM : MonoBehaviour
{
    public GameObject skillWindow;
    public GameObject hud;
    bool skillWindowup = false;
    //public int points = 10;
    public TMP_Text pointsText;


    public static int points = 0;
    public static GM instance;

    public Image healthBar;
    public Image staminaBar;
    public float health, maxHealth;
    float healthRecoverTime = 10f;
    public float stamina, maxStamina;
    public float recoverTime = 2f;



    public int publicDamage = 0;
    public int publicStamina = 0;
    public bool recoverS = true;
    public bool recoverH = true;
    float timeS = 0f;
    float timeH = 0f;

    public GameObject WinCamera;
    public GameObject WinCanvas;
    public GameObject MenuCamera;
    public GameObject MenuCanvas;

    //list to store all the game objects im gonna disable/enable when ending starting the game
    public GameObject player;
    public GameObject cams;
    public GameObject mainCamera;
    public GameObject mainHud;
    public GameObject cursor;

    void Awake()
    {
        // Ensure there is only one instance of GM
        if (instance == null)
        {
            instance = this; // Set the instance to this object
            
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GM instances
        }
    }


    void onEnable()
    {

    }

    public void killPlayer() {
        StartCoroutine(WaitFor3Secs());
    }

    IEnumerator WaitFor3Secs() {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }



    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        points = 0;
        pointsText.text = "";
        hud.SetActive(true);
        skillWindow.SetActive(false);
        health = 100;
        maxHealth = 100; ;
        maxStamina = 100f;
        stamina = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!recoverS)
        {
            timeS = 4f;
            stamina -= publicStamina;
        }
        pointsText.text = points.ToString();
        if (Input.GetKeyDown(KeyCode.P))
        {
            
            skillWindowup = !skillWindowup;
            skillWindow.SetActive(skillWindowup);
            hud.SetActive(!skillWindowup);
        }

        if (timeS <= 0)
        {
            // Stamina recovery
            if ((stamina + recoverTime) > maxStamina)

            {
                stamina = maxStamina;
            }
            else
            {
                stamina += recoverTime * Time.deltaTime;
            }
        }

        healthBar.fillAmount = health / maxHealth;
        staminaBar.fillAmount = stamina / maxStamina;

        publicDamage = 0;
        publicStamina = 0;
        recoverH = true;
        recoverS = true;
        timeH -= Time.deltaTime;
        timeS -= Time.deltaTime;

    }

    public void Win() {
        //disable the player, the two cameras, the inputs, all of it. enable ending camera
        Debug.Log("Winer");
        mainCamera.SetActive(false);
        cursor.SetActive(false);
        player.SetActive(false);
        cams.SetActive(false);
        mainHud.SetActive(false);
        WinCamera.SetActive(true);
        WinCanvas.SetActive(true);
        StartCoroutine(WaitFor3Secs());
    }

    public void MenuStart() {
        cursor.SetActive(true);
        player.SetActive(true);
        cams.SetActive(true);
        mainHud.SetActive(true);
        MenuCamera.SetActive(false);
        MenuCanvas.SetActive(false);
    }


}
