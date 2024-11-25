using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoManager : MonoBehaviour
{

    public GameObject popup;
    public Button button;



    // Start is called before the first frame update
    void Start()
    {
        popup.SetActive(false);
        button.onClick.AddListener(Pop);
    }

    void Pop() { 
        popup.SetActive(!popup.activeSelf);
    }
}
