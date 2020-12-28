using UnityEngine;
using UnityEngine.InputSystem;

public class BoardMove : MonoBehaviour {

	//Turn speed dictates how quickly the board should rotate
	public float turnSpeed = 25f;

	// Choose whether or not to show the axis wheels
	public bool showAxisWheels;

	// These are objects that indicate how much the board is turning
	public GameObject xAxisWheel;
	public GameObject zAxisWheel;

	// This just holds the input rotation provided by the OnMove event
	Vector2 inputRotation = new Vector2(0f, 0f);

	void FixedUpdate() {
		// Board Movement
		transform.Rotate(Vector3.back, inputRotation.x * turnSpeed * Time.deltaTime);
		transform.Rotate(Vector3.right, inputRotation.y * turnSpeed * Time.deltaTime);

		// Movement Indicators
		if (showAxisWheels) {
			xAxisWheel.transform.Rotate(Vector3.forward, inputRotation.x * -turnSpeed * Time.deltaTime);
			zAxisWheel.transform.Rotate(Vector3.forward, inputRotation.y * -turnSpeed * Time.deltaTime);
		}
	}

	// This is being fired by the PlayerInputs component
	public void OnMove(InputValue value) {
		inputRotation = value.Get<Vector2>();
	}

	void Update() {
		// This prevents the board from rotating on the y-axis
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
	}
}
