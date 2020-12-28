using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// See <see cref="BoardMove"/> for an updated version of this script!
/// </summary>
public class BoardController : MonoBehaviour {
	private BoardActionControls boardActionControls;

	public float turnSpeed = 25f;

	public GameObject xAxisWheel;
	public GameObject zAxisWheel;

	private void Awake() {
		boardActionControls = new BoardActionControls();
	}

	private void OnEnable() {
		boardActionControls.Enable();
	}

	private void OnDisable() {
		boardActionControls.Disable();
	}

	void Update() {
		float movementInputX = boardActionControls.Board.Tilt_X.ReadValue<float>();
		float movementInputZ = boardActionControls.Board.Tilt_Z.ReadValue<float>();

		transform.Rotate(Vector3.right, movementInputX * -turnSpeed * Time.deltaTime);
		transform.Rotate(Vector3.forward, movementInputZ * -turnSpeed * Time.deltaTime);

		xAxisWheel.transform.Rotate(Vector3.forward, movementInputX * -turnSpeed * Time.deltaTime);
		zAxisWheel.transform.Rotate(Vector3.forward, movementInputZ * -turnSpeed * Time.deltaTime);

		transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
	}
}
