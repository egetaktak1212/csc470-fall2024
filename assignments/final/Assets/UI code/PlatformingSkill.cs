using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlatformingSkill : MonoBehaviour
{
    public string skillname = "something";
    public bool purchased = false;

    public TMP_Text nameText;
    public Image skillImage;
    public GameObject player;
    public Sprite selectedSprite;
    bool changenumber = false;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = skillname;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void clicked()
    {
        if (!purchased)
        {
            if (GM.points > 0)
            {
                GM.points--;
                purchased = true;

                if (player != null)
                {
                    Debug.Log("B");
                    changeSkillIcon();
                    activate();
                }
            }
            else
            {
                Debug.Log("poor");
            }
        }
    }

    public void changeSkillIcon()
    {
        skillImage.sprite = selectedSprite;
    }

    public void activate() {
        if (skillname.Equals("Double Dash"))
        {
            player.GetComponent<PlayerControls>().maxDashes = 2;
        }
        else if (skillname.Equals("Triple Jump")) { 
            player.GetComponent<PlayerControls>().maxJumps = 3;
        }
    }

}
