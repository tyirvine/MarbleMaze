using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour {
	// These are half lengths so they can be used as a product of a (1/2) division
	[Range(10, 100)] public int gridXSizeHalfLength = 50;
	[Range(10, 100)] public int gridZSizeHalfLength = 50;

	/// <summary>Reference to y position of the parent. Used to ensure grid positions are all at the same y position.</summary>
	public int parentYPosition = 0;

	// Empties that acts as markers
	public GameObject pathFlag;
	public GameObject startFlag;
	public GameObject endFlag;
	public GameObject originFlag;

	/// <summary> Use this object to define grid positions.</summary>
	public struct GridPoints {
		public Vector3Int topLeft;
		public Vector3Int topRight;
		public Vector3Int bottomLeft;
		public Vector3Int bottomRight;

		public GameObject startPointObject;
		public GameObject endPointObject;
	}

	/// <summary>Contains all grid positions in an easy to use object.</summary>
	GridPoints gridPoints;

	// For determining whether or not to spawn the start or end point. Helps with readability
	public enum FlagAreas {
		Start,
		End
	}



	/// <summary>Sets the grid's origin point and draws an outline of it. Then it spawns both the start and end point objects and assigns them to <see cref="gridPoints"/>.</summary>
	void ConstructGrid() {
		// Find the origin grid position by inverting the gridX and gridZ lengths
		Vector3 originGridPosition = new Vector3Int(-gridXSizeHalfLength, parentYPosition, -gridZSizeHalfLength);
		GameObject originPoint = Instantiate(originFlag, originGridPosition, Quaternion.identity);

		// Simplifies grid position definitions, parent
		Vector3Int ReturnGridPoint(int x, int z) => new Vector3Int(x, parentYPosition, z);

		// Define grid positions
		gridPoints.topLeft = ReturnGridPoint(-gridXSizeHalfLength, -gridZSizeHalfLength);
		gridPoints.topRight = ReturnGridPoint(-gridXSizeHalfLength, gridZSizeHalfLength);
		gridPoints.bottomLeft = ReturnGridPoint(gridXSizeHalfLength, -gridZSizeHalfLength);
		gridPoints.bottomRight = ReturnGridPoint(gridXSizeHalfLength, gridZSizeHalfLength);

		// Duration needs to be specified, otherwise a line will only be drawn for one frame
		void DrawGridLine(Vector3Int start, Vector3Int end) => Debug.DrawLine(start, end, color: Color.white, duration: 10f);
		// Draw a rectangle of the grid
		DrawGridLine(gridPoints.topLeft, gridPoints.topRight);
		DrawGridLine(gridPoints.topRight, gridPoints.bottomRight);
		DrawGridLine(gridPoints.bottomRight, gridPoints.bottomLeft);
		DrawGridLine(gridPoints.bottomLeft, gridPoints.topLeft);


		// Finds size of either the start or end point spawn area
		Vector3Int SpawnStartEndFlag(FlagAreas flag) {
			Vector3 SpawnAreaMin;
			Vector3 SpawnAreaMax;

			Vector3 FindGridAreaMaxMin(Vector3Int referencePoint, float percentMarker) {
				// Grabs the entire domain length of the grid regardless of the grid point's signum
				int domain = Mathf.Abs(gridPoints.topLeft.z) + Mathf.Abs(gridPoints.topRight.z);
				// Makes sure to offset the Vector3.z by the topLeft.z value in order to respect the domain
				return new Vector3(referencePoint.x, 0f, (float)(domain * percentMarker - Mathf.Abs(gridPoints.topLeft.z)));
			}

			// Grab the maximum and minimum values of the spawn area specified by the flag chosen
			if (flag == FlagAreas.Start) {
				SpawnAreaMin = gridPoints.bottomLeft;
				SpawnAreaMax = FindGridAreaMaxMin(gridPoints.topRight, 0.2f);
			} else {
				SpawnAreaMin = FindGridAreaMaxMin(gridPoints.bottomRight, 0.6f);
				SpawnAreaMax = gridPoints.topRight;
			}

			// Grab random values based on spawn area's min and max points
			float spawnPointX = Random.Range(SpawnAreaMin.x, SpawnAreaMax.x);
			float spawnPointZ = Random.Range(SpawnAreaMin.z, SpawnAreaMax.z);
			// Return the position chosen!
			return new Vector3Int((int)spawnPointX, parentYPosition, (int)spawnPointZ);
		}

		/// Spawn both start flags and end flags and assign them both to the <see cref="gridPoints"/> object.
		gridPoints.startPointObject = Instantiate(startFlag, SpawnStartEndFlag(FlagAreas.Start), Quaternion.identity);
		gridPoints.endPointObject = Instantiate(endFlag, SpawnStartEndFlag(FlagAreas.End), Quaternion.identity);
	}


	// Start is called before the first frame update
	void Start() {
		ConstructGrid();
	}
}
