using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManagerTwo : MonoBehaviour

{
	// Link up to the path manager to grab the grid sizing.
	public PathManager pathManager;

	[Tooltip("this is currently for each type of corner, so this will be multiplied by 4")]
	public int numberOfCornerShapes = 10;
	/// <summary>This a list of all of the obstacle positions.</summary>
	public List<Vector3Int> obstaclePositions = new List<Vector3Int>();

	/// <summary>Determines the percentage of area the obstacles will cover.</summary>
	[Range(0.1f, 1f)] public float obstacleCoveragePercentage = 0.25f;

	// Total grid area
	[HideInInspector] public int gridArea;
	public Vector3Int gridScale;

	public int[,] nodeLayout;
	public int obstacleHeight = 0; //a value to set the obstacle height while finding lines and placing new obstacles, set as variable in case it needs to be changed later

	// Empties that acts as markers
	[Header("Object References")]
	public GameObject obstacleFlag;
	public GameObject tempObstacleFlag;


	//experimental
	
	

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
		Debug.Log("6");

		globalStaticVariables.Instance.obstacleGenerationComplete = true;
	}

	/// <summary>
	/// Check Adjacent nodes to see if they fulfil the parameters, the 2 first variables are the positive test and the second 2 should be negative to create an obstacle
	/// </summary>
	/// <param name="verticalPositionOutside"></param>
	/// <param name="horizontalPositionOutside">this value is typically 0 but we can leave it as a variable, could make for interesting results?</param>
	/// <param name="verticalPositionInside"></param>
	/// <param name="horizontalPositionInside">this value is typically 0 but we can leave it as a variable, could make for interesting results?</param>
	public void BuildCorner(int verticalPositionOutside, int horizontalPositionOutside, int verticalPositionInside, int horizontalPositionInside) // corner creator, may have to write seperate functions for different shapes
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
					pathManager.FindNodePosition(verticalPositionOutside, horizontalPositionOutside, position: currentObstaclePosition),
					pathManager.FindNodePosition(horizontalPositionOutside, verticalPositionOutside, position: currentObstaclePosition),
			};
			
			Vector3Int[] currentObstacleNeighbourPositionsEmpty = new Vector3Int[]
			{
					pathManager.FindNodePosition(verticalPositionInside, horizontalPositionInside, position: currentObstaclePosition),
					pathManager.FindNodePosition(horizontalPositionInside, verticalPositionInside, position: currentObstaclePosition),
			};

			//			-2		 2		 2
			//			-2		 1		 2
			//			-2	-1	 0	 1	 2	
			//			-2		-1		 2
			//			-2		-2		 2
			// this is how i have visualised it, possibly not the way it is working, the numbers may have to be juggled a little
			// this code creates a |_ shaped corner, to create _| or the inverse of the two, alternative functions may be required.

			

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

	/*public void CheckPositions()
    {
		Vector3Int checkNode;
		nodeLayout = new int[globalStaticVariables.Instance.gridXSizeHalfLength , globalStaticVariables.Instance.gridZSizeHalfLength ];
		Debug.Log("Size : " + globalStaticVariables.Instance.gridXSizeHalfLength );
		for(int i = 0; i < globalStaticVariables.Instance.gridXSizeHalfLength; i++)
        {
			for(int j = 0; j < globalStaticVariables.Instance.gridZSizeHalfLength; j++)
            {
				Vector3Int currentObstaclePosition = pathManager.SpawnPointInArea(PathManager.FlagAreas.Grid);
				Debug.Log("Checking node : " + (i - globalStaticVariables.Instance.gridXSizeHalfLength + 1) + "," + (j - globalStaticVariables.Instance.gridZSizeHalfLength + 1));
				//checkNode = pathManager.FindNodePosition(0, 0, position: new Vector3Int(i, 0, j));
				checkNode = pathManager.FindNodePosition(i-globalStaticVariables.Instance.gridXSizeHalfLength+1, j-globalStaticVariables.Instance.gridZSizeHalfLength+1, position: new Vector3Int(0,0,0));
				//
				if (obstaclePositions.Contains(checkNode))
				{
					nodeLayout[i, j] = 1;
					//					Debug.Log((i - globalStaticVariables.Instance.gridXSizeHalfLength) + " , " + (j - globalStaticVariables.Instance.gridZSizeHalfLength));
					Debug.Log("Node Found");
				}
				else
                {
					Debug.Log("No Node Found");
                }
			}
        }

		for (int i = 0; i < (globalStaticVariables.Instance.gridXSizeHalfLength); i++)
		{
			for (int j = 0; j < (globalStaticVariables.Instance.gridZSizeHalfLength); j++)
			{
				//Debug.Log(nodeLayout[i, j] + " ");

			}
		}
	*/
	public struct StraightLines
    {
		public Vector3Int lineStart;
		public Vector3Int lineEnd;
    }

	public StraightLines[] straightLines = new StraightLines[20];

	public void CheckPositions() //grab all the positions of Objects tagged pathObstacle
    {
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("pathObstacle");
		nodeLayout = new int[globalStaticVariables.Instance.gridXSizeHalfLength*2, globalStaticVariables.Instance.gridZSizeHalfLength*2];

		foreach(GameObject obstacle in obstacles)
        {
			int makeXNonNegative = (int)obstacle.transform.position.x + globalStaticVariables.Instance.gridXSizeHalfLength; //create offset as array cannot have a negative position
			int makeZNonNegative = (int)obstacle.transform.position.z + globalStaticVariables.Instance.gridZSizeHalfLength; //as above but for Z
			//Debug.Log("Make X : " + makeXNonNegative + " Make Z : " + makeZNonNegative);
			nodeLayout[makeXNonNegative, makeZNonNegative] = 1;
        }
		foreach(int ints in nodeLayout)
        {
		//	Debug.Log(ints);
        }
		FindStraightLines(3,2);
	}
	/// <summary>
	/// find straight lines in the grid that are of lineLength length
	/// </summary>
	/// <param name="lineLength">The number of objects in a row that would be considered a straight line</param>
	/// <param name="gapLength">The number of gaps in the line that are acceptable</param>
	public void FindStraightLines(int lineLength,int gapLength)
    {
		int currentLineLength = 0;
		int currentGapLength = 0;
		Vector3Int lineStart = new Vector3Int();
		Vector3Int lineEnd = new Vector3Int();

		for(int i = 0; i < (globalStaticVariables.Instance.gridXSizeHalfLength *2); i++)
        {
			for(int j = 0; j < (globalStaticVariables.Instance.gridZSizeHalfLength *2); j++)
            {
				if(nodeLayout[i,j] == 1 && currentGapLength < gapLength)
                {
					currentLineLength ++;
					if (currentLineLength == 1)
					{
						lineStart = new Vector3Int(i - globalStaticVariables.Instance.gridXSizeHalfLength, obstacleHeight, j - globalStaticVariables.Instance.gridZSizeHalfLength);
					}					
					if(currentLineLength == lineLength)
                    {
						lineEnd = new Vector3Int(i - globalStaticVariables.Instance.gridXSizeHalfLength, obstacleHeight, j - globalStaticVariables.Instance.gridZSizeHalfLength);
					}
                }
				else
                {
					currentGapLength++;
                }
				if(currentGapLength > gapLength)
                {
				//	Debug.Log("Line Start : " + lineStart.ToString() + " Line End : " + lineEnd.ToString());
				//	Debug.Log("Line Start : " + lineStart.ToString() + " Line End : " + lineEnd.ToString());
					lineStart = new Vector3Int(0, 0, 0);
					lineEnd = new Vector3Int(0, 0, 0);
					currentGapLength = 0;
					currentLineLength = 0;
					
					break;
                }
            }
        }
    }
}
