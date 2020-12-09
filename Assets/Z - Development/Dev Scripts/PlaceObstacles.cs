using System.Collections;
using System.Collections.Generic;
using GlobalStaticVariables;
using UnityEngine;

public class PlaceObstacles : MonoBehaviour {
	// The tile to be used for the obstacle.
	public GameObject obstacleObject;
	public int yHeight = 1;

	/// <summary>This builds the desired obstacle on the designated tile.</summary>
	public void BuildObstacles() {
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("pathObstacle");

		foreach (GameObject obstacle in obstacles) {
			Instantiate(obstacleObject, new Vector3(0, yHeight, 0) + obstacle.transform.position, obstacleObject.transform.rotation);
			if (!DebugSettings.debugModeSwitch) {
				Destroy(obstacle);
			}
		}
	}
}
