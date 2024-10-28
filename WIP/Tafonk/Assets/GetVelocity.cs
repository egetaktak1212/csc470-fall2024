using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.reddit.com/r/Unity3D/comments/15ng9b7/how_do_i_make_the_player_move_with_and_on/ i was too lazy to make it myself

public class VelocityCalculator : MonoBehaviour
{
    private Vector3 _previousPosition;
    private Vector3 _velocity;

    private void Start()
    {
        _previousPosition = transform.position;
        
    }

    private void Update()
    {

        //transform.position += transform.right * Time.deltaTime;

        _velocity = (transform.position - _previousPosition) / Time.deltaTime;
        _previousPosition = transform.position;
    }

    // player script gets the platform's velocity from here
    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        other.transform.SetParent(transform);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        other.transform.SetParent(null);
    //    }
    //}


}

