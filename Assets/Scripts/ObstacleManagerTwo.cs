using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManagerTwo : MonoBehaviour

{
	// Link up to the path manager to grab the grid sizing.
	public PathManager pathManager;

	public int numberOfCornerShapes = 10;
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
	public GameObject tempObstacleFlag;


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

	public void checkExtendedNeighbour() // corner creator, may have to write seperate functions for different shapes
    {
		int currentCount = 0;
		int failCount = 0;
		int failMax = 5000;
		while(currentCount<numberOfCornerShapes)
		{
			failCount++;
			Vector3Int currentObstaclePosition = pathManager.SpawnPointInArea(PathManager.FlagAreas.Grid);

			//check the squares 2 up and 2 across from the current position, if they are full and the inbetween squares are empty, put a square in there. 
			//et voila, corners. kinda. in the most horrible way i cound manage :D
			Vector3Int[] currentObstacleNeighbourPositions = new Vector3Int[]
			{				
					pathManager.FindNodePosition(2, 0, position: currentObstaclePosition),
					pathManager.FindNodePosition(0, 2, position: currentObstaclePosition),
			};
			
			Vector3Int[] currentObstacleNeighbourPositionsEmpty = new Vector3Int[]
			{
					pathManager.FindNodePosition(1, 0, position: currentObstaclePosition),
					pathManager.FindNodePosition(0, 1, position: currentObstaclePosition),
			};

			//			-2		 2		 2
			//			-2		 1		 2
			//			-2	-1	 0	 1	 2	
			//			-2		-1		 2
			//			-2		-2		 2
			// this is how i have visualised it, possibly not the way it is working, the numbers may have to be juggled a little

			

			//using absolute values here as we know exactly how many objects are in each array. 
			//<PSEUDO> if square 2,0 and 0,2 are already an obstacle and square 1,0 and 0,1 are empty, fill squares 1,0 and 0,1

			if(obstaclePositions.Contains(currentObstacleNeighbourPositions[0]) && obstaclePositions.Contains(currentObstacleNeighbourPositions[1]) 
			&& !obstaclePositions.Contains(currentObstacleNeighbourPositionsEmpty[0]) && !obstaclePositions.Contains(currentObstacleNeighbourPositionsEmpty[1]))
		    {					
				obstaclePositions.Add(currentObstacleNeighbourPositionsEmpty[0]);
				Instantiate(tempObstacleFlag, new Vector3(0,1,0) + Vector3.Scale(globalStaticVariables.Instance.GlobalScale, currentObstacleNeighbourPositionsEmpty[0]), Quaternion.identity);
			    obstaclePositions.Add(currentObstacleNeighbourPositionsEmpty[1]);
				Instantiate(tempObstacleFlag, new Vector3(0, 1, 0) + Vector3.Scale(globalStaticVariables.Instance.GlobalScale, currentObstacleNeighbourPositionsEmpty[1]), Quaternion.identity);

				
			//check if the square we are interrogating has an obstacle flag too, its not a corner without the cornerstone!
					Vector3Int originPoint = pathManager.FindNodePosition(0, 0, position: currentObstaclePosition);
					if (!obstaclePositions.Contains(originPoint))
					{
						Instantiate(tempObstacleFlag, new Vector3(0, 1, 0) + Vector3.Scale(globalStaticVariables.Instance.GlobalScale,originPoint), Quaternion.identity);
						//Instantiate(tempObstacleFlag, Vector3.Scale(globalStaticVariables.Instance.GlobalScale, originPoint), Quaternion.identity); //uncomment this and comment line above when its working, that just pops the cubes at Y height 1 so we can see them
						obstaclePositions.Add(originPoint);
					}

				currentCount++;
			}

			//just a brute force escape from the evil while()
			if(failCount > failMax)
            {
				currentCount = numberOfCornerShapes + 1;
            }
		}
    }
}
