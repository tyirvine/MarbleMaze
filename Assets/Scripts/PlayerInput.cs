
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
	public GameObject boardObjects;
	public float boardClamp = 20f;
	public float boardMovementDampening = 0.1f;
	Vector2 boardMovement = new Vector2(0, 0);
	public float moveSpeed = 25f;

	// Used to move the board
	void OnLook(InputValue _value) {
		Vector2 temp = _value.Get<Vector2>();
		boardMovement.x = temp.y;
		boardMovement.y = temp.x;
		if (GlobalStaticVariables.Instance.invertX) { boardMovement.y = -boardMovement.y; }
		if (GlobalStaticVariables.Instance.invertY) { boardMovement.x = -boardMovement.x; }

	}

	// Update is called once per frame
	void Update() {
		if (boardObjects) {
			boardObjects.transform.Rotate(new Vector3(boardMovement.x, 0, -boardMovement.y) * moveSpeed * Time.deltaTime);
			Vector3 rotEulers = boardObjects.transform.rotation.eulerAngles;
			// Debug.Log(rotEulers.ToString());
			rotEulers.x = (rotEulers.x <= 180 ? rotEulers.x : -(360 - rotEulers.x));
			rotEulers.x = Mathf.Clamp(rotEulers.x, -boardClamp, boardClamp);
			rotEulers.z = (rotEulers.z <= 180 ? rotEulers.z : -(360 - rotEulers.z));
			rotEulers.z = Mathf.Clamp(rotEulers.z, -boardClamp, boardClamp);
			rotEulers.y = 0;
			//transform.localEulerAngles = new Vector3(Mathf.Clamp((transform.localEulerAngles.x <= 180) ? transform.localEulerAngles.x : -(360 - transform.localEulerAngles.x), MaxDepression, MaxElevation), transform.localEulerAngles.y, transform.localEulerAngles.z);
			boardObjects.transform.eulerAngles = rotEulers;

		} else {
			boardObjects = GameObject.FindGameObjectWithTag("boardObjects");
		}
	}
}
