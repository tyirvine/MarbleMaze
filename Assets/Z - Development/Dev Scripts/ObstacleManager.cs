using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ObstacleManager : MonoBehaviour {
	/// <summary>Determines the percentage of area the obstacles will cover.</summary>
	[Header("Obstacle Settings")]
	[Range(0.1f, 2f)] public float obstacleCoveragePercentage = 0.25f;
	/// <summary>Hides or shows obstacles.</summary>
	public enum SpawnObstacles {
		None,
		ClassicObstacles,
		BuildGrid,
		BuildWall
	}
	public GameObject wallCube;
	public GameObject floorCube;

	public List<NodeObject> obstacleNodes = new List<NodeObject>();
	// This is an inspector setting for setting which obstacle type to spawn
	public SpawnObstacles spawnObstacles = SpawnObstacles.None;
	// [Header("Spawning Options")]
	public bool spawnDiagonals = false;
	// Empties that acts as markers
	[Header("Object References")]
	public GameObject obstacleFlag;
	// Link up to the path manager to grab the grid sizing.
	[Header("Script References")]
	public PathManager pathManager;
	// Total grid area
	[HideInInspector] public int gridArea;
	/// <summary>This a list of all of the obstacle positions.</summary>
	[HideInInspector] public List<Vector3Int> obstaclePositions = new List<Vector3Int>();

	/// <summary>Spawn an obstacle flag.</summary>
	public void SpawnObstacleFlag(Vector3Int currentObstaclePosition) {
		Instantiate(obstacleFlag, Vector3.Scale(GlobalStaticVariables.Instance.GlobalScale, currentObstaclePosition), Quaternion.identity);
	}

	/// <summary>This builds a wall of nodes around whatever NodeObject list is provided!</summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public List<NodeObject> AddAreaAround(Vector3Int position) {
		List<NodeObject> objects = new List<NodeObject>();
		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				objects.Add(new NodeObject(new Vector3Int(position.x + i, 0, position.z + j), 0, 0, 0, true));
			}
		}
		return objects;
	}


	#region Obstacle Generators

	// TODO: Consider deprecating
	public void ClassicObstacleGeneration() {
		// Find out grid size
		gridArea = (pathManager.gridXSizeHalfLength * 2) * (pathManager.gridZSizeHalfLength * 2);
		int obstacleCount = (int)(gridArea * obstacleCoveragePercentage);
		// Spawn specified amount of obstacles using the total area of the grid. FlagAreas.Grid ensures the entire grid is used.
		for (int i = 0; i < obstacleCount; i++) {
			while (true) {
				Vector3Int currentObstaclePosition = pathManager.SpawnPointInArea(PathManager.FlagAreas.Grid);
				// Find neighbouring points off current position
				Vector3Int[] currentObstacleNeighbourPositions = new Vector3Int[] {
					// Diagonals - 1st level
					pathManager.FindNodePosition(-1, 1, position: currentObstaclePosition),
					pathManager.FindNodePosition(1, 1, position: currentObstaclePosition),
					pathManager.FindNodePosition(1, -1, position: currentObstaclePosition),
					pathManager.FindNodePosition(-1, -1, position: currentObstaclePosition),
				};
				// Iterate through all of the neighbours to make sure no diagonals are found
				// If a diagonal position is filled break the entire loop, otherwise continue
				if (!spawnDiagonals) {
					foreach (Vector3Int neighbour in currentObstacleNeighbourPositions) {
						if (obstaclePositions.Contains(neighbour))
							goto EndOfLoop;
						else
							continue;
					}
				}
				// Check to make sure this position isn't already taken
				if (!obstaclePositions.Contains(currentObstaclePosition)) {
					obstaclePositions.Add(currentObstaclePosition);
					SpawnObstacleFlag(currentObstaclePosition);
					break;
				}
			}
		EndOfLoop:;
		}
	}


	///  <summary>Creates a list of nodes the size of the whole grid and leave out the path nodes.</summary>
	public void BuildFullGrid() {
		int gridXSizeHalfLength = pathManager.gridXSizeHalfLength;
		int gridZSizeHalfLength = pathManager.gridZSizeHalfLength;
		List<NodeObject> fullGrid = pathManager.pathNodes;
		for (int x = 1; x < gridXSizeHalfLength * 2; x++) {
			for (int y = 1; y < gridXSizeHalfLength * 2; y++) {
				if (!fullGrid.Any(s => s.position == (new Vector3Int(-gridXSizeHalfLength + x, 0, -gridZSizeHalfLength + y)))) {
					fullGrid.Add(new NodeObject(new Vector3Int(-gridXSizeHalfLength + x, 0, -gridZSizeHalfLength + y), 0, 0, 0, false));
				}
			}
		}
		foreach (NodeObject node in fullGrid) {
			Instantiate(obstacleFlag, Vector3.Scale(GlobalStaticVariables.Instance.GlobalScale, node.position), Quaternion.identity);
		}
	}

	/// <summary>This builds a wall of obstacle nodes around the path.</summary>
	public void BuildWall() {
		// Contains start and end node positions
		PathManager.GridPoints gridPoints = pathManager.gridPoints;
		// This is the path that is used to build the wall
		//List<NodeObject> path = new List<NodeObject>();

		//obstacleNodes.AddRange(pathManager.pathNodes);
		//path.AddRange(obstacleNodes);

		// Add the start and end points as nodes so that they are included in the walls
		//obstacleNodes.AddRange(AddAreaAround(gridPoints.startPointNode));
		//obstacleNodes.AddRange(AddAreaAround(gridPoints.endPointNode));


		//check through positions -1,0 1,0 0,1 0,-1 to see if there is anything present. if not, make a new node in that position and make it unwalkable
		foreach (NodeObject node in pathManager.pathNodes) {

			Vector3Int[] checkNeighbours = pathManager.FindNodeNeighbours(node.position, 1);

			foreach (Vector3Int position in checkNeighbours) {
				// This wants a node that isn't in the path nodes and isn't in the obstacle nodes, it then will take a node like that and add it to obstacle nodes
				//if (!path.Any(nodes => nodes.position == position) && !obstacleNodes.Any(nodes => nodes.position == position)) {
				if (!pathManager.pathNodes.Any(nodes => nodes.position == position)) {
					obstacleNodes.Add(new NodeObject(position, 0, 0, 0, false));
					SpawnObstacleFlag(position);
				}
			}


		}
	}

	#endregion

	/// <summary>Picks which obstacle type to spawn.</summary>
	public void ObstaclePicker() {
		switch (spawnObstacles) {
		case SpawnObstacles.None:
			break;
		case SpawnObstacles.ClassicObstacles:
			ClassicObstacleGeneration();
			break;
		case SpawnObstacles.BuildGrid:
			BuildFullGrid();
			break;
		case SpawnObstacles.BuildWall:
			BuildWall();
			break;
		}
	}
}