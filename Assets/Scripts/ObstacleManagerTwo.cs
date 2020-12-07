using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManagerTwo : MonoBehaviour

{
	// Link up to the path manager to grab the grid sizing.
	public PathManager pathManager;

	/// <summary>This a list of all of the obstacle positions.</summary>
	public List<Vector3Int> obstaclePositions = new List<Vector3Int>();

	/// <summary>Determines the percentage of area the obstacles will cover.</summary>
	[Range(0.1f, 1f)] public float obstacleCoveragePercentage = 0.25f;

	// Total grid area
	[HideInInspector] public int gridArea;
	public Vector3Int gridScale;

	// Empties that acts as markers
	[Header("Object References")]
	public GameObject obstacleFlag;


	public void GenerateObstacleMap()
	{
		// Find out grid size
		gridArea = (globalStaticVariables.Instance.gridXSizeHalfLength * 2) * (globalStaticVariables.Instance.gridZSizeHalfLength * 2);
		int obstacleCount = (int)(gridArea * obstacleCoveragePercentage);

		// Spawn specified amount of obstacles using the total area of the grid. FlagAreas.Grid ensures the entire grid is used.
		for (int i = 0; i < obstacleCount; i++)
		{
			while (true)
			{
				Vector3Int currentObstaclePosition = pathManager.SpawnPointInArea(PathManager.FlagAreas.Grid);
				// Find neighbouring points off current position
				Vector3Int[] currentObstacleNeighbourPositions = new Vector3Int[] {
					pathManager.FindNodePosition(-1, 1, position: currentObstaclePosition),
					pathManager.FindNodePosition(1, 1, position: currentObstaclePosition),
					pathManager.FindNodePosition(1, -1, position: currentObstaclePosition),
					pathManager.FindNodePosition(-1, -1, position: currentObstaclePosition)
				};

				// Iterate through all of the neighbours to make sure no diagonals are found
				// If a diagonal position is filled break the entire loop, otherwise continue
				foreach (Vector3Int neighbour in currentObstacleNeighbourPositions)
				{
					if (obstaclePositions.Contains(neighbour))
						goto EndOfLoop;
					else
						continue;
				}

				// Check to make sure this position isn't already taken
				if (!obstaclePositions.Contains(currentObstaclePosition))
				{
					obstaclePositions.Add(currentObstaclePosition);
					Instantiate(obstacleFlag, Vector3.Scale(globalStaticVariables.Instance.GlobalScale, currentObstaclePosition), Quaternion.identity);
					break;
				}
			}
			EndOfLoop:;
		}
	}
}
