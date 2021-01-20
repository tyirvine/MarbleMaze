
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;

[Serializable]
public class ShapeTemplate {
	[Header("Shape Management")]
	[Tooltip("Insert the premade shape here, in the default orientation")]
	public GameObject rule;
	public GameObject model;
	public int unitCount;               //how many Units make this object
	public bool includeInBuild = true;
	public bool rotate = true;
}

///create a class to store each child point in a shape, this can be done from the child transform, but this method is a little tidier
///as it doesnt rely on checking a transform.tag each time. 
public class ShapePoints {
	public Vector3Int position;
	public bool invalid;
	public int mode; //this can be 1, 2 or 3, based on the "tag" of the object, shapePlace, shapeInvalid, shapeDontPlace
}

public class ShapeManager : MonoBehaviour {
	public ShapeTemplate[] shapes;
	//completed checks can be removed once we have the execution order sorted and this script runs in the correct sequence.
	bool completedChecks = false;

	[Header("Draw Flags of obstacle positions")]
	public bool spawnFlags = false;
	public GameObject obstacleFlag;
	int rotation = 90;
	//update this once execution order is complete, this is an awful method for executing scripts :D
	//  private void Update() { if (!completedChecks && GlobalStaticVariables.Instance.obstacleGenerationComplete) { CheckShapes(); } }



	public void CheckShapes() {
		List<NodeObject> pathNodes = new List<NodeObject>();
		completedChecks = true;
		pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().obstacleNodes);
		List<Vector3Int> placedPositions = new List<Vector3Int>();
		List<Vector3Int> nodePositions = new List<Vector3Int>();
		List<ShapePoints> shapePoints = new List<ShapePoints>();
		List<Vector3Int> walkNodes = new List<Vector3Int>();

		// assign the unwalkable pathNodes to nodePositions and the walkable pathNodes to pathPositions
		for (int i = 0; i < pathNodes.Count; i++) {
			if (!pathNodes[i].walkable) {
				nodePositions.Add(Vector3Int.RoundToInt(pathNodes[i].position));
				if (spawnFlags) {
					Instantiate(obstacleFlag, pathNodes[i].position, Quaternion.identity);
				}
			}
			if (pathNodes[i].walkable) {
				walkNodes.Add(Vector3Int.RoundToInt(pathNodes[i].position));
			}
		}
		Debug.Log("WALKNODES : " + walkNodes.Count);
		Array.Sort(shapes, delegate (ShapeTemplate x, ShapeTemplate y) { return x.unitCount.CompareTo(y.unitCount); });
		Array.Reverse(shapes);

		foreach (ShapeTemplate currentShape in shapes) {
			if (currentShape.includeInBuild) {
				GameObject tempRule = Instantiate(currentShape.rule);
				foreach (Vector3Int nodePosition in nodePositions) {
					if (!placedPositions.Contains(nodePosition)) {
						int rotation = 1;
						if (currentShape.rotate) {
							rotation = 4;
						}
						for (int i = 0; i < rotation; i++) {
							shapePoints.Clear();
							tempRule.transform.position = nodePosition;
							tempRule.transform.Rotate(0, i * 90, 0);

							foreach (Transform childObject in tempRule.transform) {
								ShapePoints tempShape = new ShapePoints();
								tempShape.position = Vector3Int.RoundToInt(childObject.transform.position);
								switch (childObject.tag) {
								case "shapeValid": tempShape.mode = 1; break;
								case "shapeDontPlace": tempShape.mode = 2; break;
								case "shapeInvalid": tempShape.mode = 3; break;
								case "shapeNoPath": tempShape.mode = 4; break;
								default: tempShape.mode = 3; break;
								}
								shapePoints.Add(tempShape);
							}

							int count = 0;
							foreach (ShapePoints checkPosition in shapePoints) {
								if (checkPosition.mode == 4 && walkNodes.Contains(checkPosition.position)) {
									count = -200; //silly value, probably a better way to ensure failure
									break;
								}
								if (checkPosition.mode == 3 && nodePositions.Contains(checkPosition.position)) {
									count = -200; //silly value, probably a better way to ensure failure
									break;
								}
								if (checkPosition.mode == 1 || checkPosition.mode == 2) {
									if (nodePositions.Contains(checkPosition.position) && !placedPositions.Contains(checkPosition.position))
										count++;
								}

							}

							if (count == currentShape.unitCount) //have we got enough positive hits to build this object? if so, make the model and add the points to the placedPositions list so they are unavailable
							{
								Instantiate(currentShape.model, tempRule.transform.position, tempRule.transform.rotation);
								foreach (ShapePoints placePos in shapePoints) {
									if (placePos.mode == 1) {
										placedPositions.Add(placePos.position);
									}
								}
							}
						}
						Destroy(tempRule); // this stops the rule being visible at the end of the object generation
					}
				}//if currentshape include in build

			}

		}
	}
}
