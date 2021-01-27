using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour {

	// These are half lengths so they can be used as a product of a (1/2) division
//	[Header("Grid Settings")]
//	[Range(7, 100)] public int gridXSizeHalfLength = 10;
//	[Range(7, 100)] public int gridZSizeHalfLength = 10;

	// Scaling for the placement of objects on the grid
	Vector3 gridScale;

	/// <summary>This dictates how much of the grid area should be start node spawnable area.</summary>
//	[Range(0f, 0.2f)] public float startAreaPercentage = 0.1f;

//	[Header("Start/End point settings")]
//	[Range(2, 8)] public int cornerLengthDivider = 2;

	// Decides how long the path itself should be, measured in integral units.
	[Header("Path Settings")]
	[Range(6, 100)] public int desiredPathLength = 6;

	/// <summary>Creates a randomized path if enabled.</summary>
//	[Header("Path Manipulation")]
//	public bool isPathWacky = false;
//	[Range(1, 40)] public int wackiness;

	// Flag controls
	[Header("Flag Control")]
	public bool disablePathFlags = false;
	// Build Controls
	[Header("Build Controls")]
	public bool buildBoard = false;
	public bool buildShapes = false;
	public bool buildObstacles = false;
	
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
	//int gridDrawDuration = 1000;

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
	//public class CornerNode {
	//	public Vector3Int position;
	//	public Corner corner;
	//	public CornerNode(Vector3Int position, Corner corner) {
	//		this.position = position;
	//		this.corner = corner;
//		}
//	}

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
			// All - extended perimeter
			case 5:
				return new Vector3Int[] {
					// Non-diagonals
					FindNodePosition(-2, 0, position),
					FindNodePosition(2, 0, position),
					FindNodePosition(0, 2, position),
					FindNodePosition(0, -2, position),
					// Diagonals
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
		
	}

	


	/// <summary>Finds all the clearance nodes for the provided position. Top, top right, and right side.</summary>
	public Vector3Int[] FindClearanceNodes(Vector3Int position) {
		return new Vector3Int[] {
				FindNodePosition(0, 1, position),
				FindNodePosition(1, 1, position),
				FindNodePosition(1, 0, position),
			};
	}

	/// <summary>This back tracks from the current node to find the starting node, making a path.</summary>
	public void RetracePath(NodeObject lastNode) {
		NodeObject traceNode = lastNode;

		/// <summary>This links the child node to the parent node by spawning nodes inbetween.</summary>
		void AddPathNodeAtOffset(NodeObject node, String axis, int offset) {
			Vector3Int newNodePos;
			Vector3Int newNodePosSecond;
			if (axis == "x") {
				newNodePos = new Vector3Int(node.position.x, parentYPosition, node.position.z + offset);
				newNodePosSecond = new Vector3Int(node.position.x, parentYPosition, node.position.z + (offset * 2));
			} else {
				newNodePos = new Vector3Int(node.position.x + offset, parentYPosition, node.position.z);
				newNodePosSecond = new Vector3Int(node.position.x + (offset * 2), parentYPosition, node.position.z);
			}
			pathNodes.Add(new NodeObject(newNodePos));
			pathNodes.Add(new NodeObject(newNodePosSecond));
		}

		// Link all nodes by creating a new node between the parent and child
		foreach (NodeObject node in closedNodes) {
			if (node.parent != null) {
				// Check if both nodes are on the x-axis
				if (node.position.x == node.parent.position.x) {
					if (node.position.z > node.parent.position.z) {
						AddPathNodeAtOffset(node, "x", -1);
					} else {
						AddPathNodeAtOffset(node, "x", 1);
					}
				}
				// Check if both nodes are on the z-axis
				else if (node.position.z == node.parent.position.z) {
					if (node.position.x > node.parent.position.x) {
						AddPathNodeAtOffset(node, "z", -1);
					} else {
						AddPathNodeAtOffset(node, "z", 1);
					}
				}
			}
			// Add current pathnode
			pathNodes.Add(node);
		}

		// // Builds a trace by taking in the parent node of each node and adding it to the path nodes list
		// while (traceNode.position != gridPoints.startPointNode) {
		// 	if (traceNode.position != gridPoints.endPointNode) pathNodes.Add(traceNode);
		// 	traceNode = traceNode.parent;
		// }

		// // Add in both start and end points
		// pathNodes.Add(new NodeObject(gridPoints.startPointNode));
		// pathNodes.Add(new NodeObject(gridPoints.endPointNode));

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
		if (!disablePathFlags) {
			foreach (NodeObject node in pathNodes) {
				Instantiate(pathFlag, node.position, Quaternion.identity);
			}
			Instantiate(startFlag, gridPoints.startPointNode, Quaternion.identity);
			Instantiate(endFlag, gridPoints.endPointNode, Quaternion.identity);
		}
	}

	/// <summary>This checks to see if the point collides with any non-pathable positions.</summary>
	
	/* -------------------------------------------------------------------------- */
	/*                               Path Generation                              */
	/* -------------------------------------------------------------------------- */

	/// <summary>This handles the creation of a path from the start point to the end point!</summary>
	void GeneratePath() {
		// Grab current position for starting point
		//gridPoints.startPointNode = currentWorldPosition;

		// Add the start node to the open points list
		//openNodes.Add(new NodeObject(gridPoints.startPointNode, 0, 0, 0, false));

		// This object contains the current node being investigated
		NodeObject currentNode = new NodeObject(gridPoints.startPointNode);

		// This variable keeps track of how much progress it's made in getting to the desired path length
		int pathLengthProgress = 0;

		// Loop Emergency Break
		int loopEmergencyBrake = 0;
		int loopEmergencyBrakeCap = 5000;

		// This loops until a path is generated from the start node to the end node
		while (loopEmergencyBrake < loopEmergencyBrakeCap) {

			if (pathLengthProgress > 0) {
				// Add previous node (known as current) to the closed nodes
				closedNodes.Add(currentNode);
				// Instantiate(startFlag, currentNode.position, startFlag.transform.rotation);
				// Randomly pick the next node out of the open nodes and assign that as the new current
				int randomOpenNode = Random.Range(0, openNodes.Count());
				currentNode = openNodes[randomOpenNode];
				// Empty out open nodes list
				openNodes.Clear();
			}
			// Path incremented one position
			pathLengthProgress++;

			// TODO: Rewrite this to account for path length instead
			// Check to see if the current node position is equal to the end or target node's position
			// and log the current position as the end position
			if (pathLengthProgress >= desiredPathLength) {
				gridPoints.endPointNode = currentNode.position;
				closedNodes.Add(currentNode);
				RetracePath(currentNode);
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
			new NodeObject(FindNodePosition(-3, 0, currentNode: currentNode), 0, 0, 0,false),
			new NodeObject(FindNodePosition(0, 3, currentNode: currentNode), 0, 0, 0,false),
			new NodeObject(FindNodePosition(3, 0, currentNode: currentNode), 0, 0, 0,false),
			new NodeObject(FindNodePosition(0, -3, currentNode: currentNode), 0, 0, 0,false)
			};

			// Loop through all neighbours
			foreach (NodeObject node in neighbourNodes) {
				// Check to see if the current neighbour node is intersecting with a closed node
				if (closedNodes.Any(nodes => nodes.position == node.position)) {
					continue;
				}

				// Otherwise add that node to a list of possible directions to take
				node.parent = currentNode;
				openNodes.Add(node);
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

	// Restart from here
	RestartLoop:;

#if UNITY_EDITOR
		// Used to time path construction
		long loopStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		GlobalStaticVariables.Instance.DebugLogEntry("Started PathManager.cs");
#endif

		// Initialize
		Initialize();
		GameObject[] flags;

		// Destroy all flags first
		flags = GameObject.FindGameObjectsWithTag("Flag");
		foreach (GameObject flag in flags) GameObject.Destroy(flag);
		GameObject[] walls = GameObject.FindGameObjectsWithTag("wallTile");
		foreach (GameObject wall in walls) {
			Destroy(wall);
		}

		// This catch is looking for a `No sequence` error that can occur when the path can't go from start to finish
		try {
			// Generates the entire path
			GeneratePath();
			if (buildObstacles) obstacleManager.BuildWall();
		} catch (Exception e) {
			Debug.LogException(e, this);
			Debug.LogWarning("Error caught - Loop Reset");
			errorCaught = true;
			goto RestartLoop;
		}
		// A valid path has been generated!
		if (errorCaught) Debug.Log("Error resolved - Loop Completed!");

		// Build walls
		// TODO: This is causing errors
		if (buildShapes) gameObject.GetComponent<ShapeManager>().CheckShapes();
		if (buildBoard) gameObject.GetComponent<BuildBoard>().GetBoardSize();

		// TODO: Do we need this? - Ty @bubzy-coding
		//	GameObject.FindGameObjectWithTag("shapeManager").GetComponent<ShapeManager>().gameObject.SetActive(true); //heckShapesAgainstObstacles();
		//Instantiate(shapeManager, transform.position, Quaternion.identity);

#if UNITY_EDITOR
		// Path construction finished
		long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		string currentTimeLog = "Path found in " + (currentTime - loopStartTime) + "ms!";
		GlobalStaticVariables.Instance.DebugLogEntry("Finished PathManager.cs - " + currentTimeLog);
		Debug.Log(currentTimeLog);
#endif

	}

	// Start is called before the first frame updates
	 void Start() {
	 	PublicStart();

	 }

	public void PublicStart() {
		// Grab parameters from global variables
		gridScale = GlobalStaticVariables.Instance.GlobalScale;
		openNodes = new List<NodeObject>();
		closedNodes = new List<NodeObject>();
		pathNodes = new List<NodeObject>();
		clearanceNodes = new List<NodeObject>();
		// Executes the entire path stack
		ConstructPathStack();
		// GlobalStaticVariables.Instance.pathGenerationComplete = true;

		// Collects all the flags in the scene and parents them
		if (GlobalStaticVariables.Instance.collectFlags) {
			GameObject[] flags = GameObject.FindGameObjectsWithTag("Flag");
			GameObject flagParent = new GameObject(name: "FlagParent");

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
	public void OnFire()
    {
	//	ConstructPathStack();
    }
	public void OnDebug() {
	//	ConstructPathStack();
	}
}

/*// WARNING - This function must execute after contruct grid. If it executes prior there will be errors.
//create start and end nodes, ensure that they spawn apart in seperate quadrants of the grid
//divide grid: top left - 0, top right - 1, bottom left - 2, bottom right - 3. make a function to randomly pick one for the start and ensure that the diagonally opposite contains the end.
void SpawnStartEnd() {
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
	if (!disablePathFlags) Instantiate(startFlag, Vector3.Scale(gridScale, gridPoints.startPointNode), Quaternion.identity);
	if (!disablePathFlags) Instantiate(endFlag, Vector3.Scale(gridScale, gridPoints.endPointNode), Quaternion.identity);
}

/// <summary>Sets the grid's origin point and draws an outline of it. Then it spawns both the start and end point objects and assigns them to <see cref="gridPoints"/>.</summary>
void ConstructGrid()
{
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

public bool IsPathable(NodeObject node) {
		// Grab clearance neighbours to provided position
		Vector3Int[] positionClearanceNeighbours = FindNodeNeighbours(node.position, 5);
		bool isPathable = false;

		// Start by making sure the point is within the grid bounds
		foreach (Vector3Int position in positionClearanceNeighbours)
			if (CheckIfInGridBounds(position))
				isPathable = true;
			else
				return false;

		return isPathable;
	}

	/// <summary>Grabs the difference in distance of both the x & z points between Node A and Node B.</summary>
	public int GetDistance(NodeObject nodeA, NodeObject nodeB) {
		int dstX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
		int dstZ = Mathf.Abs(nodeA.position.z - nodeB.position.z);
		if (!isPathWacky) {
			// Bubzy do you know what's happening here?
			if (dstX > dstZ)
				return 14 * dstZ + 10 * (dstX - dstZ);
			else
				return 14 * dstX + 10 * (dstZ - dstX);
		}
		// and here?
		if (dstX > dstZ) return Random.Range(1, wackiness) * dstZ + 10 * (dstX - dstZ);
		return Random.Range(1, wackiness) * dstX + 10 * (dstZ - dstX);
	}

	// =========================================
	// Phase 2: Generate the path
	// =========================================

	/// <summary>Check to see if a position is within the grid bounds or not.</summary>
//	public bool CheckIfInGridBounds(Vector3Int position) {
	//	GridPoints grid = gridPoints;
		//if ((position.z < grid.topLeft.z && position.z > grid.bottomRight.z) && (position.x < grid.bottomRight.x && position.x > grid.topLeft.x))
	//		return true;
//		else
	//		return false;
//	}
*/