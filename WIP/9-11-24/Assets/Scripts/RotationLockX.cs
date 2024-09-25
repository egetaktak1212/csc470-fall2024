using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLockX : MonoBehaviour
{
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //alright i need to write this here so i remember it later: Here, i made this empty to copy the position of the parent. This makes it move with the
        //plane. However, I only want it to rotate on one axis with the plane, the other axis should stay constant. So, here I'm trying to set the rotation of
        //the empty to the rotation of the plane ONLY in the left right turn axis. Now, i have no idea which axis that is, hence, testing.
        transform.position = parent.transform.position;

        transform.rotation = Quaternion.Euler(parent.transform.rotation.eulerAngles.x, parent.transform.rotation.eulerAngles.y, 0);

        //0, parent.transform.rotation.y, 0
    }

}
