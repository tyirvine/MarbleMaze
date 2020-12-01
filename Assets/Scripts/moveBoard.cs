﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBoard : MonoBehaviour {

	//Turn speed dictates how quickly the board should rotate
	public float turnSpeed = 25f;

	void FixedUpdate() {
		//transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);

		//These two move on the Z-Axis
		if (Input.GetKey(KeyCode.UpArrow))
			transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.DownArrow))
			transform.Rotate(Vector3.forward, turnSpeed * Time.deltaTime);

		//These two move on the X-Axis
		if (Input.GetKey(KeyCode.LeftArrow))
			transform.Rotate(Vector3.right, turnSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.RightArrow))
			transform.Rotate(Vector3.left, turnSpeed * Time.deltaTime);
	}

}
