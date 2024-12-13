using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSkillTree : MonoBehaviour
{

    public GameObject disableOnUnlock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        disableOnUnlock.SetActive(false);
    }

    private void OnDisable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        disableOnUnlock.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
