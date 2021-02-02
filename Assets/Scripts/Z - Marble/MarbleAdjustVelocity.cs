using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleAdjustVelocity : MonoBehaviour
{
    //made non public as the parameters no longer need to be assigned in inspector.
    Rigidbody marbleRigidbody;
    private void Start()
    {
        marbleRigidbody = GetComponent<Rigidbody>();
        //make it so that there is no limit to the speed the marble can go, this allows the marble to 
        //roll as fast as it can along a corrider etc.
        marbleRigidbody.maxAngularVelocity = Mathf.Infinity;
    }

    private void FixedUpdate()
    {
        //set the marble to never rest, this avoids it "sticking" to walls etc
        marbleRigidbody.WakeUp();
    }
}
