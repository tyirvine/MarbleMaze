using UnityEngine;

public class FinishHoleIsEntered : MonoBehaviour {
	[HideInInspector] public bool holeIsEngaged;

	private void OnTriggerEnter(Collider other) {
		holeIsEngaged = true;
		Debug.Log("FinishHoleIsEntered: Player Finished Board!");
	}
}
