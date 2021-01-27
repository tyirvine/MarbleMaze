using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleAdjustVelocity : MonoBehaviour {

	// We're just grabbing a reference to the marble's rigidbody so we can set it's max angular velocity.
	public Rigidbody marbleRigidbody;
	[Tooltip("The default max angular velocity is 7 and it feels a bit slow at times.")]
	public float maxAngularVelocity = 30f;
	[Tooltip("The default sleep threshold is 0.005.")]
	public float sleepThreshold = 0.005f;

	private void Start() {
		// Here we're just setting the maximum angular velocity of the rigidbody to [maxAngularVelocity]
		//marbleRigidbody.maxAngularVelocity = maxAngularVelocity;
		// This decides whether or not to stop doing physics calculations on the rigidbody
		marbleRigidbody.sleepThreshold = sleepThreshold;
		
	//	Debug.Log(marbleRigidbody.velocity.y.ToString());
		
	}

    private void FixedUpdate()
    {
	

	}
}
