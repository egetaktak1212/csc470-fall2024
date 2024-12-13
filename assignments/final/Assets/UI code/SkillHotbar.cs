using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine.Samples;
using UnityEngine;
using UnityEngine.UI;

public class SkillHotbar : MonoBehaviour
{
    public TMP_Text numberText;
    int number;
    public static Action<SkillHotbar> UnSelectUI;
    public Image skillImage;
    public Sprite noSkillSprite;
    public Sprite skillSprite;
    public Sprite selectedSprite;
    public GameObject shootManager = null;
    SimplePlayerShoot shootScript;
    SlashSkill slash;
    SpinSkill spin;
    public bool open = false;
    // Start is called before the first frame update
    void Start()
    {
  
        number = Convert.ToInt32(numberText.text);
        skillImage.sprite = noSkillSprite;

        //if this is for fireball, specifically make it fireball script. I know this sucks ok
        if (number == 1)
        {
            shootScript = shootManager.GetComponent<SimpleFireball>();
        }
        else if (number == 2)
        {
            shootScript = shootManager.GetComponent<SimplePlayerShoot>();
        }
        else if (number == 3)
        {
            slash = shootManager.GetComponent<SlashSkill>();
        }
        else if (number == 4) { 
            spin = shootManager.GetComponent<SpinSkill>();
        }

        if (open)
        {
            setOpen();
        }
    }

    private void OnEnable()
    {
        UnSelectUI += unSelectUI;
    }

    void unSelectUI(SkillHotbar obj) {
        if (obj != this) {
            unselectSkill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode[] list = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        if (Input.GetKeyDown(list[number - 1]) && open) {
            selectSkill();
            UnSelectUI?.Invoke(this);

        }

        Debug.Log(open);


    }

    public void Select(bool select) {
        if (number < 3)
        {
            shootScript.selected = select;
        }
        else if (number == 3)
        {
            slash.selected = select;
        }
        else if (number == 4) { 
            spin.selected = select;
        }
    }


    public void selectSkill()
    {
        if (open && shootManager != null)
        {
            skillImage.sprite = selectedSprite;
            Select(true);
        }
    }

    public void unselectSkill() {
        if (open && shootManager != null) {
            skillImage.sprite = skillSprite;
            Select(false);
        }

    }

    public void setOpen() {
        skillImage.sprite = skillSprite;
        open = true;
    }



}
