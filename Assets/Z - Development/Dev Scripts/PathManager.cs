using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour {
	// These are half lengths so they can be used as a product of a (1/2) division
	[Header("Grid Size")]
	[Range(10, 100)] public int gridXSizeHalfLength = 50;
	[Range(10, 100)] public int gridZSizeHalfLength = 50;

	/// <summary>Reference to y position of the parent. Used to ensure grid positions are all at the same y position.</summary>
	public int parentYPosition = 0;

	/// <summary>How long the grid should appear drawn for. This int gets casted as a float automatically.</summary>
	[Header("Draw Time")]
	[Range(0, 100)] public int gridDrawDuration = 10;

	// Empties that acts as markers
	[Header("Object References")]
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

		public Vector3Int startPointNode;
		public Vector3Int endPointNode;
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
		void DrawGridLine(Vector3Int start, Vector3Int end) => Debug.DrawLine(start, end, color: Color.white, duration: gridDrawDuration);
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

		// Assign the start and end point node positions
		gridPoints.startPointNode = SpawnStartEndFlag(FlagAreas.Start);
		gridPoints.endPointNode = SpawnStartEndFlag(FlagAreas.End);

		// Spawn both start flags and end flags
		Instantiate(startFlag, gridPoints.startPointNode, Quaternion.identity);
		Instantiate(endFlag, gridPoints.endPointNode, Quaternion.identity);
	}


	// Node object to keep track of path cost
	public class NodeObject {
		public Vector3Int position;
		/// <summary>Distance from the starting node.</summary>
		public int gCost;
		/// <summary>Distance from the end node.</summary>
		public int hCost;
		/// <summary>G cost + H cost.</summary>
		public int fCost;

		public NodeObject parent;

		/// <summary>Calculates the G cost + H cost.</summary>
		public void AssignFCost(GridPoints gridPoints) {
			// Finds the distance between the current node and the starting node.
			gCost = (int)(position - gridPoints.startPointNode).magnitude;
			// Finds the distance between the current node and the end node.
			hCost = (int)(position - gridPoints.endPointNode).magnitude;
			fCost = gCost + hCost;
		}

		public NodeObject(Vector3Int position, int gCost, int hCost, int fCost) {
			this.position = position;
			this.gCost = gCost;
			this.hCost = hCost;
			this.fCost = fCost;
		}
	}


	/// <summary>This handles the creation of a path from the start point to the end point!</summary>
	void GeneratePath() {
		/// For an explination on what these node lists mean please visit ⤵
		/// https://www.notion.so/scriptobit/Environment-Path-Generation-a5304e8f37474efa98809a03f0e26074
		List<NodeObject> openNodes = new List<NodeObject>();
		List<NodeObject> closedNodes = new List<NodeObject>();
		List<NodeObject> pathNodes = new List<NodeObject>();

		// Add the start node to the open points list
		openNodes.Add(new NodeObject(gridPoints.startPointNode, 0, 0, 0));

		// This object contains the current node being investigated
		NodeObject currentNode;

		/// <summary>Check to see if a position is within the grid bounds or not.</summary>
		bool CheckIfInGridBounds(Vector3Int position) {
			GridPoints grid = gridPoints;
			if ((position.z < grid.topRight.z && position.z > grid.bottomLeft.z) && (position.x < grid.bottomLeft.x && position.x > grid.topRight.x))
				return true;
			else
				return false;
		}

		/// <summary>Check to see if the new path to specified node is shorter than the previously stored path.</summary>
		bool IsNewPathShorter(NodeObject newNode) {
			// This finds our stored node by searching through the open nodes list based on position
			NodeObject storedNode = openNodes.Find(nodes => nodes.position == newNode.position);
			newNode.AssignFCost(gridPoints);
			// Then check to see if the stored node's fcost is higher than the new node's
			if (newNode.fCost < storedNode.fCost) {
				return true;
			} else
				return false;
		}

		/// <summary>This back tracks from the current node to find the starting node, making a path.</summary>
		void RetracePath(NodeObject lastNode) {
			NodeObject traceNode = lastNode;
			while (traceNode.position != gridPoints.startPointNode) {
				pathNodes.Add(traceNode);
				traceNode = traceNode.parent;
			}
			// Reverse the list because we started tracing from the end
			pathNodes.Reverse();
			// Instantiate desired object
			foreach (NodeObject node in pathNodes)
				Instantiate(pathFlag, node.position, Quaternion.identity);
		}

		// TODO: Remove this, it's just for testing
		long loopStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();


		// This loops until a path is generated from the start node to the end node
		while (true) {
			// Find node with the lowest f_cost, remove it from the open nodes and add it to the closed nodes
			int lowestFCost = openNodes.Min(nodes => nodes.fCost);
			currentNode = openNodes.First(nodes => nodes.fCost == lowestFCost);
			openNodes.Remove(currentNode);
			closedNodes.Add(currentNode);

			// Check to see if the current node position is equal to the end or target node's position
			if (currentNode.position == gridPoints.endPointNode) {
				// TODO: Remove this, it's just for testing --- ⤵
				long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
				Debug.Log("Path found in " + (currentTime - loopStartTime) + "ms!");
				// -------------------------------------------- ⤴
				RetracePath(currentNode);
				return;
			}

			// Find the current node's neighbours
			/// Below is which index corresponds to which neighbour
			///				(-x)
			///				0
			///	(-z)	3		1	(+z)
			///				2
			///				(+x)
			NodeObject[] neighbourNodes = new NodeObject[4];
			// Initializing the array with empty nodeobjects
			for (int i = 0; i < neighbourNodes.Length; i++)
				neighbourNodes[i] = new NodeObject(Vector3Int.zero, 0, 0, 0);

			/// <summary>Simplify finding node position.</summary>
			Vector3Int FindNodePosition(int xOffset, int zOffset) {
				Vector3Int referencePosition = currentNode.position;
				return new Vector3Int(referencePosition.x + xOffset, parentYPosition, referencePosition.z + zOffset);
			}

			// Assign all neighbour node positions
			neighbourNodes[0].position = FindNodePosition(-1, 0);
			neighbourNodes[1].position = FindNodePosition(0, 1);
			neighbourNodes[2].position = FindNodePosition(1, 0);
			neighbourNodes[3].position = FindNodePosition(0, -1);

			// Loop through all neighbours
			foreach (NodeObject node in neighbourNodes) {
				// Checks to see if the current neighbour node being investigated is in the closedNodes list or traversable
				if (closedNodes.Any(nodes => nodes.position == node.position) || !CheckIfInGridBounds(node.position))
					// If it is then skip this loop iteration and move to the next neighbour node
					continue;
				// Otherwise check to see if the node is in the open list or if the new path to the node is shorter than the stored path
				if (!openNodes.Any(nodes => nodes.position == node.position) || (IsNewPathShorter(node))) {
					node.AssignFCost(gridPoints);
					node.parent = currentNode;
					// If the node is not found in the open nodes list then add it
					if (!openNodes.Any(nodes => nodes.position == node.position))
						openNodes.Add(node);
				}
			}
		}
	}


	// Start is called before the first frame update
	void Start() {
		ConstructGrid();
		GeneratePath();
	}
}
