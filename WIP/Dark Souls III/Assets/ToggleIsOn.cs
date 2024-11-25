using cakeslice;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
    UnityEngine.UI.Toggle m_Toggle;
    public Text m_Text;
    GameObject main;
    public string option;


    void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponent<UnityEngine.UI.Toggle>();
        //Add listener for when the state of the Toggle changes, to take action
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged();
        });
        main = gameObject.transform.parent.gameObject.transform.parent.gameObject;
    }

    private void OnDisable()
    {
       DeSelected();
    }



    private void Update()
    {
        //Debug.Log(m_Toggle.isOn);
        //Debug.Log(main);
    }

    public void Selected() {

        main.GetComponent<UnitScript>().options[option] = true;
    }

    public void DeSelected()
    {

        main.GetComponent<UnitScript>().options[option] = false;

    }

    void ToggleValueChanged()
    {
        //main.GetComponent<UnitScript>().options["move"] = m_Toggle.isOn;
    }
}