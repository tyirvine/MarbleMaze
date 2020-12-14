using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMove : MonoBehaviour {

	//Turn speed dictates how quickly the board should rotate
	public float turnSpeed = 25f;

	void FixedUpdate() {
		// Controlled by the Input Manager found in Project Settings > Input Manager
		transform.Rotate(Vector3.back, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime);
		transform.Rotate(Vector3.right, Input.GetAxis("Vertical") * turnSpeed * Time.deltaTime);
	}

	void Update() {
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
	}

}
