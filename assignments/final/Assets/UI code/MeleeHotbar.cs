using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine.Samples;
using UnityEngine;
using UnityEngine.UI;

public class MeleeHotbar : MonoBehaviour
{
    public TMP_Text numberText;
    int number;
    public static Action<MeleeHotbar> UnSelectMeleeUI;
    public Image skillImage;
    public Sprite noSkillSprite;
    public Sprite skillSprite;
    public Sprite selectedSprite;
    public GameObject shootManager = null;
    SimplePlayerShoot shootScript;
    bool open = false;
    // Start is called before the first frame update
    void Start()
    {

        number = Convert.ToInt32(numberText.text);
        skillImage.sprite = noSkillSprite;

        //if this is for fireball, specifically make it fireball script. I know this sucks ok
        if (number == 1) {
            shootScript = shootManager.GetComponent<SimplePlayerShoot>();
        } else if (number == 2)
        {
            shootScript = shootManager.GetComponent<SimplePlayerShoot>();
        }

    }

    private void OnEnable()
    {
        UnSelectMeleeUI += unSelectUI;
    }

    void unSelectUI(MeleeHotbar obj) {
        if (obj != this) {
            unselectSkill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode[] list = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        if (Input.GetKeyDown(list[number - 1])) {
            selectSkill();
            UnSelectMeleeUI?.Invoke(this);

        }


    }

    public void selectSkill()
    {
        if (open && shootManager != null)
        {
            skillImage.sprite = selectedSprite;
            shootScript.selected = true;
        }
    }

    public void unselectSkill() {
        if (open && shootManager != null) {
            skillImage.sprite = skillSprite;
            shootScript.selected = false;
        }

    }

    public void setOpen() {
        Debug.Log("A");
        skillImage.sprite = skillSprite;
        open = true;
    }



}
