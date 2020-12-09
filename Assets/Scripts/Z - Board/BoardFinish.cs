using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFinish : MonoBehaviour {
	public Transform destination;
	public float distance = 1f;

	// Update is called once per frame
	void Update() {
		if (Vector3.Distance(transform.position, destination.position) <= distance) {
			Debug.Log("You Have Beat the Level");
		}
	}
}
