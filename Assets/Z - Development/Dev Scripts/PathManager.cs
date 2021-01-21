using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour {

	// These are half lengths so they can be used as a product of a (1/2) division
	[Header("Grid Settings")]
	[Range(7, 100)] public int gridXSizeHalfLength = 10;
	[Range(7, 100)] public int gridZSizeHalfLength = 10;

	// Scaling for the placement of objects on the grid
	Vector3 gridScale;

	/// <summary>This dictates how much of the grid area should be start node spawnable area.</summary>
	[Range(0f, 0.2f)] public float startAreaPercentage = 0.1f;

	[Header("Start/End point settings")]
	[Range(2, 8)] public int cornerLengthDivider = 2;

	// Decides how long the path itself should be, measured in integral units.
	[Header("Path Settings")]
	[Range(0.1f, 1.0f)] public float desiredPathLengthPercentage = 0.1f;

	/// <summary>Creates a randomized path if enabled.</summary>
	[Header("Path Manipulation")]
	public bool isPathWacky = false;
	[Range(1, 40)] public int wackiness;

	// Empties that acts as markers
	[Header("Object References")]
	public GameObject pathFlag;
	public GameObject startFlag;
	public GameObject endFlag;

	// Reference to the obstacle manager for linking up the obstacle position list and generating obstacle maps
	[Header("Script References")]
	public ObstacleManager obstacleManager;

	/// <summary>Use this to determine if a path has been succesfully generated or not.</summary>
	[HideInInspector] public bool didPathGenerate;

	/// <summary>Provide the current position of the grid origin.</summary>
	[HideInInspector] public Vector3Int currentWorldPosition { get => Vector3Int.FloorToInt(gameObject.transform.position); }
	/// <summary>Reference to y position of the parent. Used to ensure grid positions are all at the same y position.</summary>
	// [HideInInspector] public int parentYPosition = 0;
	[HideInInspector] public int parentYPosition { get => currentWorldPosition.y; }

	/// <summary>How long the grid should appear drawn for. This int gets casted as a float automatically.</summary>
	int gridDrawDuration = 1000;

	/// For an explination on what these node lists mean please visit ⤵
	/// https://www.notion.so/scriptobit/Environment-Path-Generation-a5304e8f37474efa98809a03f0e26074
	public List<NodeObject> openNodes = new List<NodeObject>();
	public List<NodeObject> closedNodes = new List<NodeObject>();
	public List<NodeObject> pathNodes = new List<NodeObject>();
	public List<NodeObject> clearanceNodes = new List<NodeObject>();

	/// <summary> Use this object to define grid positions.</summary>
	public struct GridPoints {
		// Grid corners
		public Vector3Int topLeft;
		public Vector3Int topRight;
		public Vector3Int bottomLeft;
		public Vector3Int bottomRight;

		public Vector3Int startPointNode;
		public Vector3Int endPointNode;

		/// <summary>Use this to store the start and end points.</summary>
		public List<Vector3Int> placedPoints;
	}

	/// <summary>Contains all grid positions in an easy to use object.</summary>
	public GridPoints gridPoints;

	//a buffer to stop start and endpoints spawning too close to each other;

	/// <summary>An object that contains which corner and position it's at.</summary>
	public class CornerNode {
		public Vector3Int position;
		public Corner corner;
		public CornerNode(Vector3Int position, Corner corner) {
			this.position = position;
			this.corner = corner;
		}
	}

	// For determining whether or not to spawn the start or end point. Helps with readability
	public enum FlagAreas {
		Start,
		End,
		Grid
	}

	// Used to determine which corner
	public enum Corner {
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	// =========================================
	// Global Functions & Objects
	// =========================================

	/// <summary>Simplify finding node position.
	/// <code>
	///	<br/>
	///.............(+z) <br/>
	///.............+1 <br/>
	///..(-x)..-1........+1...(+x) <br/>
	///.............-1 <br/>
	///.............(-z) <br/>
	///</code>
	/// </summary>
	public Vector3Int FindNodePosition(int xOffset, int zOffset, [Optional] Vector3Int position, NodeObject currentNode = null) {
		// Below is a ternary operator, here's a link if you're not familiar → https://bit.ly/39O0q8e
		Vector3Int referencePosition = currentNode == null ? position : currentNode.position;
		return new Vector3Int(referencePosition.x + xOffset, parentYPosition, referencePosition.z + zOffset);
	}

	/// <summary>Simplifies finding the neighbour positions of a node.</summary>
	/// <example>mode 1 is diagonals, mode 2 is non diagonals</example>
	public Vector3Int[] FindNodeNeighbours(Vector3Int position, int mode) {
		switch (mode) {
			// Diagonals
			case 0:
				return new Vector3Int[] {
					FindNodePosition(-1, -1, position),
					FindNodePosition(1, 1, position),
					FindNodePosition(-1, 1, position),
					FindNodePosition(1, -1, position),
					};
			// All
			case 1:
				return new Vector3Int[] {
					FindNodePosition(-1, 0, position),
					FindNodePosition(0, 1, position),
					FindNodePosition(1, 0, position),
					FindNodePosition(0, -1, position),

					FindNodePosition(-1, 1, position),
					FindNodePosition(1, 1, position),
					FindNodePosition(-1, -1, position),
					FindNodePosition(1, -1, position),
				   };
			// Non-Diagonals
			case 2:
				return new Vector3Int[] {
					FindNodePosition(-1, 0, position),
					FindNodePosition(0, 1, position),
					FindNodePosition(1, 0, position),
					FindNodePosition(0, -1, position),
				   };
			// Sides
			case 3:
				return new Vector3Int[] {
					FindNodePosition(-2, 0, position),
					FindNodePosition(2, 0, position),
				   };
			// All including center
			case 4:
				return new Vector3Int[] {
					position,

					FindNodePosition(-1, 0, position),
					FindNodePosition(0, 1, position),
					FindNodePosition(1, 0, position),
					FindNodePosition(0, -1, position),

					FindNodePosition(-1, 1, position),
					FindNodePosition(1, 1, position),
					FindNodePosition(-1, -1, position),
					FindNodePosition(1, -1, position),
				   };
			// All - two levels
			case 5:
				return new Vector3Int[] {
					FindNodePosition(-1, 0, position),
					FindNodePosition(0, 1, position),
					FindNodePosition(1, 0, position),
					FindNodePosition(0, -1, position),

					FindNodePosition(-1, 1, position),
					FindNodePosition(1, 1, position),
					FindNodePosition(-1, -1, position),
					FindNodePosition(1, -1, position),

					// 2nd level
					FindNodePosition(-2, 0, position),
					FindNodePosition(2, 0, position),
					FindNodePosition(0, 2, position),
					FindNodePosition(0, -2, position),

					// Inbetweeners
					FindNodePosition(1, 2, position),
					FindNodePosition(1, -2, position),
					FindNodePosition(-1, 2, position),
					FindNodePosition(-1, -2, position),

					FindNodePosition(2, 1, position),
					FindNodePosition(2, -1, position),
					FindNodePosition(-2, 1, position),
					FindNodePosition(-2, -1, position),

					FindNodePosition(-2, 2, position),
					FindNodePosition(2, 2, position),
					FindNodePosition(-2, -2, position),
					FindNodePosition(2, -2, position),
				   };
		}
		return null;
	}

	// =========================================
	// Phase 1: Construct the grid
	// =========================================

	/// <summary>Ensures anything that needs to be reset gets reset.</summary>
	void Initialize() {
		gridPoints = new GridPoints { placedPoints = new List<Vector3Int>() };
		didPathGenerate = false;

		openNodes = new List<NodeObject>();
		closedNodes = new List<NodeObject>();
		pathNodes = new List<NodeObject>();
		clearanceNodes = new List<NodeObject>();

		// Obstacle manager
		obstacleManager.obstaclePositions = new List<Vector3Int>();
		obstacleManager.gridArea = 0;
	}

	// TODO: Scrap this function
	/// <summary>Simplifies spawning a point in a specified area.</summary>
	public Vector3Int SpawnPointInArea(FlagAreas flag) {
		// Finds size of either the start or end point spawn area
		Vector3 SpawnAreaMin;
		Vector3 SpawnAreaMax;

		// Simplifies finding a spawn area's max and min Vector3s
		Vector3 FindGridAreaMaxMin(Vector3Int referencePoint, float percentMarker) {
			// Grabs the entire domain length of the grid regardless of the grid point's signum
			int domain = Mathf.Abs(gridPoints.bottomLeft.z) + Mathf.Abs(gridPoints.topLeft.z);
			// Makes sure to offset the Vector3.z by the topLeft.z value in order to respect the domain
			return new Vector3(referencePoint.x, 0f, (float)(domain * percentMarker - Mathf.Abs(gridPoints.bottomLeft.z)));
		}

		// Grab the maximum and minimum values of the spawn area specified by the flag chosen
		if (flag == FlagAreas.Start) {
			SpawnAreaMin = gridPoints.bottomRight;
			SpawnAreaMax = FindGridAreaMaxMin(gridPoints.topLeft, startAreaPercentage);
		}
		// Spawns just for the end area
		else if (flag == FlagAreas.End) {
			float remainingArea = 1f - (startAreaPercentage * 2);
			SpawnAreaMin = FindGridAreaMaxMin(gridPoints.topRight, remainingArea);
			SpawnAreaMax = gridPoints.topLeft;
		}
		// Spawns for the entire grid area
		else {
			SpawnAreaMin = gridPoints.bottomRight;
			SpawnAreaMax = gridPoints.topLeft;
		}

		// Grab random values based on spawn area's min and max points
		float spawnPointX = Random.Range(SpawnAreaMin.x, SpawnAreaMax.x);
		float spawnPointZ = Random.Range(SpawnAreaMin.z, SpawnAreaMax.z);
		// Return the position chosen!
		return new Vector3Int((int)spawnPointX, parentYPosition, (int)spawnPointZ);
	}

	/// <summary>Sets the grid's origin point and draws an outline of it. Then it spawns both the start and end point objects and assigns them to <see cref="gridPoints"/>.</summary>
	void ConstructGrid() {
		// Simplifies grid position definitions, parent
		Vector3Int ReturnGridPoint(int x, int z) => new Vector3Int(x, 0, z) + currentWorldPosition;

		// Define grid positions
		gridPoints.topLeft = ReturnGridPoint(-gridXSizeHalfLength, gridZSizeHalfLength);
		gridPoints.topRight = ReturnGridPoint(gridXSizeHalfLength, gridZSizeHalfLength);
		gridPoints.bottomLeft = ReturnGridPoint(-gridXSizeHalfLength, -gridZSizeHalfLength);
		gridPoints.bottomRight = ReturnGridPoint(gridXSizeHalfLength, -gridZSizeHalfLength);

		// Duration needs to be specified, otherwise a line will only be drawn for one frame
		void DrawGridLine(Vector3Int start, Vector3Int end) => Debug.DrawLine(start, end, color: Color.white, duration: gridDrawDuration);
		// Draw a rectangle of the grid
		DrawGridLine(gridPoints.bottomLeft, gridPoints.topLeft);
		DrawGridLine(gridPoints.topLeft, gridPoints.topRight);
		DrawGridLine(gridPoints.topRight, gridPoints.bottomRight);
		DrawGridLine(gridPoints.bottomRight, gridPoints.bottomLeft);
	}

	// =========================================
	// Phase 2: Generate the path
	// =========================================

	/// <summary>Check to see if a position is within the grid bounds or not.</summary>
	public bool CheckIfInGridBounds(Vector3Int position) {
		GridPoints grid = gridPoints;
		if ((position.z < grid.topLeft.z && position.z > grid.bottomRight.z) && (position.x < grid.bottomRight.x && position.x > grid.topLeft.x))
			return true;
		else
			return false;
	}

	// TODO: Scrap this function
	/// <summary>This spawns the start and end points by making sure they have ample room and aren't colliding.</summary>
	void SpawnStartOrEnd(FlagAreas flag, GameObject flagObject) {
		// TODO: This function needs to be cleaned up, it's no longer dealing with obstacles
		/// <summary>This keeps track of the loop and will fire off a warning to reset the obstacle gen if it's taking too long.</summary>
		int loopCounter = 0;
		// Loop through until a valid spawn point is found
		while (loopCounter <= obstacleManager.gridArea) {
			// Generate spawn point based on area
			Vector3Int possibleSpawn = SpawnPointInArea(FlagAreas.Grid);
			// Check to see if the possible spawn is colliding with the obstacle positions
			if (!obstacleManager.obstaclePositions.Contains(possibleSpawn) && !gridPoints.placedPoints.Contains(possibleSpawn)) {
				// Find all neighbours of the possible spawn point

				List<Vector3Int> possibleSpawnNeighbours = new List<Vector3Int>();
				for (int x = -2; x <= 2; x++) {
					for (int y = -2; y <= 2; y++) {
						if (x != 0 && y != 0) {
							possibleSpawnNeighbours.Add(FindNodePosition(x, y, position: possibleSpawn));
						}
					}
				}

				// Verifies that the neighbouring positions are also not colliding with obstacle positions
				// if a collision is detected it breaks out of the loop so it can try another spawn point
				foreach (Vector3Int neighbour in possibleSpawnNeighbours) {
					if (obstacleManager.obstaclePositions.Contains(neighbour) || gridPoints.placedPoints.Contains(neighbour))
						goto EndOfLoop;
					else
						continue;
				}
				// All checks are successful so the flag can be spawned!
				if (flag == FlagAreas.Start) gridPoints.startPointNode = possibleSpawn;
				if (flag == FlagAreas.End) gridPoints.endPointNode = possibleSpawn;
				Instantiate(flagObject, Vector3.Scale(gridScale, possibleSpawn), Quaternion.identity);

				// Makes itself a placed point to avoid overlap with other points
				gridPoints.placedPoints.Add(possibleSpawn);

				return;
			}
		// The goto jumps to here
		EndOfLoop:;

			// If loop is close to firing tell the obstacle builder to rebuild
			if (loopCounter < obstacleManager.gridArea) {
				didPathGenerate = false;
			} else
				loopCounter++;
		}
	}

	// WARNING - This function must execute after contruct grid. If it executes prior there will be errors.
	//create start and end nodes, ensure that they spawn apart in seperate quadrants of the grid
	//divide grid: top left - 0, top right - 1, bottom left - 2, bottom right - 3. make a function to randomly pick one for the start and ensure that the diagonally opposite contains the end.
	void AltStartEnd() {
		int cornerLength = 0;
		///leave a gap at 1/4 of the size of a quadrant. this does reduce the amount of space available to a start/end point
		///this takes the length of the shortest side and divides it by the corner length divider. This gives us 
		///a corner side that is nice and short. Resulting in nice square areas to spawn our points.
		if (gridXSizeHalfLength < gridZSizeHalfLength) {
			cornerLength = (int)(gridXSizeHalfLength / cornerLengthDivider);
		} else {
			cornerLength = (int)(gridZSizeHalfLength / cornerLengthDivider);
		}

		// This function simplifies picking out a random position within a corner
		Vector3Int PickOutCornerPosition(int xMinDirection, int xMaxDirection, int zMinDirection, int zMaxDirection) {
			return new Vector3Int(GlobalStaticVariables.Instance.RandomEven(xMinDirection, xMaxDirection), parentYPosition, GlobalStaticVariables.Instance.RandomEven(zMinDirection, zMaxDirection));
		}

		/// This is a function that calculates the corner length's position in world space.
		/// 1 = +x / +z
		/// -1 = -x / -z
		int FindCornerLengthPosition(int direction, bool isXAxis) {
			if (isXAxis)
				return currentWorldPosition.x + (cornerLength * direction);
			else
				return currentWorldPosition.z + (cornerLength * direction);
		}

		// These functions define the corners
		Vector3Int TopLeft() => PickOutCornerPosition(gridPoints.topLeft.x, FindCornerLengthPosition(-1, true), FindCornerLengthPosition(1, false), gridPoints.topLeft.z);
		Vector3Int TopRight() => PickOutCornerPosition(FindCornerLengthPosition(1, true), gridPoints.topRight.x, FindCornerLengthPosition(1, false), gridPoints.topRight.z);
		Vector3Int BottomLeft() => PickOutCornerPosition(gridPoints.bottomLeft.x, FindCornerLengthPosition(-1, true), FindCornerLengthPosition(-1, false), gridPoints.bottomLeft.z);
		Vector3Int BottomRight() => PickOutCornerPosition(FindCornerLengthPosition(1, true), gridPoints.bottomRight.x, gridPoints.bottomRight.z, FindCornerLengthPosition(-1, false));

		// Randomly pick out a corner to start at
		int startQuad = Mathf.RoundToInt(Random.Range(0, 4));
		switch (startQuad) {
			case 0:             //top left
				gridPoints.startPointNode = TopLeft();
				gridPoints.endPointNode = BottomRight();
				break;
			case 1:             //top right
				gridPoints.startPointNode = TopRight();
				gridPoints.endPointNode = BottomLeft();
				break;
			case 2:             //bottom left
				gridPoints.startPointNode = BottomLeft();
				gridPoints.endPointNode = TopRight();
				break;
			case 3:             //bottom right
				gridPoints.startPointNode = BottomRight();
				gridPoints.endPointNode = TopLeft();
				break;
		}

		// Spawn start / end flags
		Instantiate(startFlag, Vector3.Scale(gridScale, gridPoints.startPointNode), Quaternion.identity);
		Instantiate(endFlag, Vector3.Scale(gridScale, gridPoints.endPointNode), Quaternion.identity);
	}

	/// <summary>This handles the creation of a path from the start point to the end point!</summary>
	void GeneratePath() {
		// Spawn start and end points
		//SpawnStartOrEnd(FlagAreas.Start, startFlag);
		//SpawnStartOrEnd(FlagAreas.End, endFlag);
		AltStartEnd();
		// Add the start node to the open points list
		openNodes.Add(new NodeObject(gridPoints.startPointNode, 0, 0, 0, false));

		// This object contains the current node being investigated
		NodeObject currentNode;

		/// <summary>Finds all the clearance nodes for the provided position. Top, top right, and right side.</summary>
		Vector3Int[] FindClearanceNodes(Vector3Int position) {
			return new Vector3Int[] {
				// First level
				FindNodePosition(0, 1, position),
				FindNodePosition(1, 1, position),
				FindNodePosition(1, 0, position),

				// // Second level
				// FindNodePosition(0, 2, position),
				// FindNodePosition(2, 2, position),
				// FindNodePosition(2, 0, position),

				// // Inbetweeners
				// FindNodePosition(1, 2, position),
				// FindNodePosition(2, 1, position)

			};
		}

		/// <summary>This back tracks from the current node to find the starting node, making a path.</summary>
		void RetracePath(NodeObject lastNode) {
			NodeObject traceNode = lastNode;

			// Builds a trace by taking in the parent node of each node and adding it to the path nodes list
			while (traceNode.position != gridPoints.startPointNode) {
				if (traceNode.position != gridPoints.endPointNode) pathNodes.Add(traceNode);
				traceNode = traceNode.parent;
			}

			// Add in both start and end points
			pathNodes.Add(new NodeObject(gridPoints.startPointNode));
			pathNodes.Add(new NodeObject(gridPoints.endPointNode));

			// // Setup an extra list so the for loop doesn't grow ↴
			NodeObject[] pathNodesClearance = pathNodes.ToArray();
			// Add in clearance nodes to path nodes
			foreach (NodeObject node in pathNodesClearance) {
				Vector3Int[] clearancePositions = FindClearanceNodes(node.position);
				foreach (Vector3Int position in clearancePositions)
					pathNodes.Add(new NodeObject(position, 0, 0, 0, true));
			}

			// Reverse the list because we started tracing from the end, and calculate the path's length
			pathNodes.Reverse();

			// Instantiate desired object
			foreach (NodeObject node in pathNodes) {
				Instantiate(pathFlag, Vector3.Scale(gridScale, node.position), Quaternion.identity);
			}

			// foreach (NodeObject node in clearanceNodes) {
			// 	Instantiate(startFlag, Vector3.Scale(gridScale, node.position), Quaternion.identity);
			// }

			// foreach (NodeObject node in closedNodes) {
			// 	Instantiate(endFlag, Vector3.Scale(gridScale, node.position), Quaternion.identity);
			// }

		}

		/// <summary>This checks to see if the point collides with any non-pathable positions.</summary>
		bool IsPathable(NodeObject node) {
			// Grab clearance neighbours to provided position
			Vector3Int[] positionClearanceNeighbours = FindNodeNeighbours(node.position, 1);
			bool isPathable = false;

			// Start by making sure the point is within the grid bounds
			foreach (Vector3Int position in positionClearanceNeighbours)
				if (CheckIfInGridBounds(position))
					isPathable = true;
				else
					return false;

			// Check to see if there is enough clearance to make a turn
			// Vector3Int[] diagonals = FindNodeNeighbours(node.position, 0);
			// If there's a diagonal hit on two corners it's not pathable
			// if (clearanceNodes.Any(nodes => nodes.position == diagonals[0]) && clearanceNodes.Any(nodes => nodes.position == diagonals[1]))
			// 	return false;
			// else if (clearanceNodes.Any(nodes => nodes.position == diagonals[2]) && clearanceNodes.Any(nodes => nodes.position == diagonals[3]))
			// 	return false;
			// int diagonalCount = 0;
			// foreach (Vector3Int diagonal in diagonals) {
			// 	Debug.Log(diagonalCount);
			// 	if (diagonalCount >= 2)
			// 		return false;
			// 	if (clearanceNodes.Any(nodes => nodes.position == diagonal))
			// 		diagonalCount++;
			// }

			// Grab corner positions
			// CornerNode[] pathNodeCorners = new CornerNode[] {
			// 	new CornerNode(FindNodePosition(-1, -1, node.position), Corner.BottomLeft),
			// 	new CornerNode(FindNodePosition(-1, 1, positionClearanceNeighbours[0]), Corner.TopLeft),
			// 	new CornerNode(FindNodePosition(1, 1, positionClearanceNeighbours[1]), Corner.TopRight),
			// 	new CornerNode(FindNodePosition(1, -1, positionClearanceNeighbours[2]), Corner.BottomRight)
			// };

			// // Checks a corner node's neighbours
			// bool ContainsAdjacentNodes(Corner corner, Vector3Int position) {
			// 	Vector3Int adjacentOne = Vector3Int.zero;
			// 	Vector3Int adjacentTwo = Vector3Int.zero;

			// 	// Finds adjacent corner positions depending on corner
			// 	switch (corner) {
			// 		case Corner.TopLeft:
			// 			adjacentOne = FindNodePosition(0, -1, position);
			// 			adjacentTwo = FindNodePosition(1, 0, position);
			// 			break;
			// 		case Corner.TopRight:
			// 			adjacentOne = FindNodePosition(-1, 0, position);
			// 			adjacentTwo = FindNodePosition(0, -1, position);
			// 			break;
			// 		case Corner.BottomLeft:
			// 			adjacentOne = FindNodePosition(0, 1, position);
			// 			adjacentTwo = FindNodePosition(1, 0, position);
			// 			break;
			// 		case Corner.BottomRight:
			// 			adjacentOne = FindNodePosition(-1, 0, position);
			// 			adjacentTwo = FindNodePosition(0, 1, position);
			// 			break;
			// 	}

			// 	// Checks to see if closed nodes contains either of the adjacent nodes
			// 	if (closedNodes.Any(nodes => nodes.position == adjacentOne) || closedNodes.Any(nodes => nodes.position == adjacentTwo))
			// 		return true;
			// 	// Otherwise that means we found a diagonal so...
			// 	else
			// 		return false;
			// }

			// // Then check each corner to make sure it's not a diagonal pinch
			// foreach (CornerNode cornerNode in pathNodeCorners)
			// 	if (closedNodes.Any(nodes => nodes.position == cornerNode.position)) {
			// 		// If it does contain a node on the corner, then it checks for adjacents
			// 		if (!ContainsAdjacentNodes(cornerNode.corner, cornerNode.position))
			// 			return false;
			// 		else
			// 			isPathable = true;
			// 	} else
			// 		isPathable = true;

			return isPathable;
		}

		/// <summary>Grabs the difference in distance of both the x & z points between Node A and Node B.</summary>
		int GetDistance(NodeObject nodeA, NodeObject nodeB) {
			int dstX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
			int dstZ = Mathf.Abs(nodeA.position.z - nodeB.position.z);
			if (!isPathWacky) {
				// Bubzy do you know what's happening here?
				if (dstX > dstZ)
					return 14 * dstZ + 10 * (dstX - dstZ);
				else
					return 14 * dstX + 10 * (dstZ - dstX);
				// if (dstX > dstZ)
				// 	return 14 * dstZ + 10 * (dstX - dstZ);
				// else
				// 	return 14 * dstX + 10 * (dstZ - dstX);
			}
			// and here?
			if (dstX > dstZ) return Random.Range(1, wackiness) * dstZ + 10 * (dstX - dstZ);
			return Random.Range(1, wackiness) * dstX + 10 * (dstZ - dstX);
		}

		// Adds a new set of clearance nodes
		int clearanceCounter = 0;

		// Loop Emergency Break
		int loopEmergencyBrake = 0;
		int loopEmergencyBrakeCap = 5000;

		// This loops until a path is generated from the start node to the end node
		while (loopEmergencyBrake < loopEmergencyBrakeCap) {

			// Find node with the lowest f_cost, remove it from the open nodes and add it to the closed nodes
			int highestFCost = openNodes.Max(nodes => nodes.fCost);
			currentNode = openNodes.First(nodes => nodes.fCost == highestFCost);
			openNodes.Remove(currentNode);
			closedNodes.Add(currentNode);

			// Check to see if the current node position is equal to the end or target node's position
			if (currentNode.position == gridPoints.endPointNode) {
				RetracePath(currentNode);
				didPathGenerate = true;
				return;
			}

			// Find the current node's neighbours
			/// Below is which index corresponds to which neighbour
			///				(+z)
			///				0
			///	(-x)	3		1	(+x)
			///				2
			///				(-z)
			// Assign all neighbour node positions
			NodeObject[] neighbourNodes = new NodeObject[] {
			new NodeObject(FindNodePosition(-1, 0, currentNode: currentNode), 0, 0, 0,false),
			new NodeObject(FindNodePosition(0, 1, currentNode: currentNode), 0, 0, 0,false),
			new NodeObject(FindNodePosition(1, 0, currentNode: currentNode), 0, 0, 0,false),
			new NodeObject(FindNodePosition(0, -1, currentNode: currentNode), 0, 0, 0,false)
			};

			// Loop through all neighbours
			foreach (NodeObject node in neighbourNodes) {
				// Checks to see if the current neighbour node being investigated is in the closedNodes list or is traversable
				if (closedNodes.Any(nodes => nodes.position == node.position) || !IsPathable(node))
					// If it is then skip this loop iteration and move to the next neighbour node
					continue;
				// Find the longest distance
				int newNeighbourMovementCost = currentNode.gCost + GetDistance(currentNode, node);
				// Otherwise check to see if the node is in the open list or if the new path to the node is shorter than the stored path
				if (newNeighbourMovementCost < node.gCost || !openNodes.Any(nodes => nodes.position == node.position)) {
					node.gCost = newNeighbourMovementCost;
					node.hCost = GetDistance(node, new NodeObject(gridPoints.endPointNode, 0, 0, 0, false));
					node.parent = currentNode;

					// Add clearance neighbours as well
					if (clearanceCounter >= 2) {
						if (currentNode.parent != null && currentNode.parent.parent != null && currentNode.parent.parent.parent != null) {
							Vector3Int[] clearanceNeighbours = FindNodeNeighbours(currentNode.parent.parent.parent.position, 5);
							foreach (Vector3Int position in clearanceNeighbours)
								closedNodes.Add(new NodeObject(position));

							clearanceCounter = 0;
						} else
							clearanceCounter = 0;
					}
					clearanceCounter++;

					// If the node is not found in the open nodes list then add it
					if (!openNodes.Any(nodes => nodes.position == node.position)) {

						openNodes.Add(node);

					}
				}
			}
			// Acts as an emergency break for this loop
			loopEmergencyBrake++;
		}
		// Reports if this loop is functioning correctly or not
		if (loopEmergencyBrake > loopEmergencyBrakeCap) Debug.LogError("Path generation loop broken!");
	}

	// =========================================
	// Phase 3: Construct Everything
	// =========================================

	/// <summary>This will spawn the grid, obstacle positions, and path positions. It checks to make sure the path is valid,
	/// if it detects that the path is not valid it reruns, starting at RestartLoop. Usually it only takes one rerun
	/// to generate a valid path.</summary>
	void ConstructPathStack() {
		// A bool switch to see if an error was caught on the try carch
		bool errorCaught = false;
		int emergencyBrake = 0;

	// Restart from here
	RestartLoop:;
		// Initialize
		Initialize();
		GameObject[] flags;

		// Stop game if emergency brake is engaged
		emergencyBrake++;
		if (emergencyBrake > 100) {
			UnityEditor.EditorApplication.isPlaying = false;
			Application.Quit();
		}

		// Destroy all flags first
		flags = GameObject.FindGameObjectsWithTag("Flag");
		foreach (GameObject flag in flags) GameObject.Destroy(flag);
		GameObject[] walls = GameObject.FindGameObjectsWithTag("wallTile");
		foreach (GameObject wall in walls) {
			Destroy(wall);
		}

		// Build the grid and spawn the obstacles
		ConstructGrid();

		// This catch is looking for a `No sequence` error that can occur when the path can't go from start to finish
		try {
			// Generates the entire path
			GeneratePath();
			obstacleManager.ObstaclePicker();
		} catch (Exception e) {
			Debug.LogException(e, this);
			Debug.LogWarning("Error caught - Loop Reset");
			errorCaught = true;
			// TODO: Reinstate
			goto RestartLoop;
		}
		// A valid path has been generated!
		if (errorCaught) Debug.Log("Error resolved - Loop Completed!");

		// TODO: Repair
		//gameObject.GetComponent<ShapeManager>().CheckShapes();
		//gameObject.GetComponent<BuildBoard>().GetBoardSize();
		// TODO: Do we need this? - Ty @bubzy-coding
		//	GameObject.FindGameObjectWithTag("shapeManager").GetComponent<ShapeManager>().gameObject.SetActive(true); //heckShapesAgainstObstacles();
		//Instantiate(shapeManager, transform.position, Quaternion.identity);

	}

	// Start is called before the first frame updates
	void Start() {
#if UNITY_EDITOR
		long loopStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		GlobalStaticVariables.Instance.DebugLogEntry("Started PathManager.cs");
#endif

		// Grab parameters from global variables
		gridScale = GlobalStaticVariables.Instance.GlobalScale;

		// Executes the entire path stack
		ConstructPathStack();
		GlobalStaticVariables.Instance.pathGenerationComplete = true;

#if UNITY_EDITOR
		long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		string currentTimeLog = "Path found in " + (currentTime - loopStartTime) + "ms!";
		GlobalStaticVariables.Instance.DebugLogEntry("Finished PathManager.cs - " + currentTimeLog);
		Debug.Log(currentTimeLog);
#endif

		// Collects all the flags in the scene and parents them
		if (GlobalStaticVariables.Instance.collectFlags) {
			GameObject[] flags = GameObject.FindGameObjectsWithTag("Flag");
			GameObject flagParent = new GameObject();

			foreach (GameObject flag in flags) {
				flag.transform.SetParent(flagParent.transform);
			}
		}

	}

	// TODO: Delete this - this is only for testing
	bool constructPath = false;
	void Update() {
		if (constructPath) {
			ConstructPathStack();
			constructPath = false;
		}
	}

	public void OnDebug() {
		constructPath = true;
	}
}
