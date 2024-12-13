using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skill : MonoBehaviour
{
    public int number = 0;
    public string skillname = "something";
    public bool purchased = false;
    public float staminaCost = 10;

    public TMP_Text nameText;
    public TMP_Text numberText;
    public Image skillImage;
    public GameObject UIElement = null;
    //public GameObject game;
    
    bool changenumber = false;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = skillname;
        if (!purchased)
        {
            numberText.text = "";
        }
        else {
            numberText.text = number.ToString();
            UIElement.GetComponent<SkillHotbar>().setOpen();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void clicked()
    {
        if (!purchased) {
            if (GM.points > 0)
            {
                GM.points--;
                purchased = true;

                numberText.text = number.ToString();
                if (UIElement != null)
                {
                    Debug.Log("B");
                    UIElement.GetComponent<SkillHotbar>().setOpen();
                }
            }
            else {
                Debug.Log("poor");
            }
        }

        if (purchased == true)
        {
            this.changenumber = true;
        }
        return;
    }

    public void setNumber(int number)
    {
        this.number = number;
    }
    public void setPurchased(bool purchase)
    {
        this.purchased = purchase;
    }
}
