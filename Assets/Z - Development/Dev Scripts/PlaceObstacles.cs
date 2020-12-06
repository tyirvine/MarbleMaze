using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObstacles : MonoBehaviour {
	// The tile to be used for the obstacle.
	public GameObject obstacleObject;

	/// <summary>This builds the desired obstacle on the designated tile.</summary>
	public void BuildObstacles() {
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("pathObstacle");
		foreach (GameObject obstacle in obstacles) {
			Instantiate(obstacleObject, obstacle.transform.position, obstacleObject.transform.rotation);
			Destroy(obstacle);
		}
	}
}
