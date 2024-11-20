using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{

    public string unitName;
    public string bio;
    public string stats;

    public NavMeshAgent nma;

    public Renderer bodyRenderer;
    public Color normalColor;
    public Color selectedColor;

    public Vector3 destination;

    public bool selected = false;

    float rotateSpeed;

    LayerMask layerMask;

    void OnEnable()
    {
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
            bodyRenderer.material.color = normalColor;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("wall");

        GameManager.instance.units.Add(this);

        rotateSpeed = Random.Range(20, 60);

        transform.Rotate(0, Random.Range(0, 360), 0);
    }

    void OnDestroy()
    {
        GameManager.instance.units.Remove(this);
    }

    

    // Update is called once per frame
    void Update()
    {

    }

}