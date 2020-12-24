using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ObstacleManager : MonoBehaviour
{
    /// <summary>Determines the percentage of area the obstacles will cover.</summary>
    [Header("Obstacle Settings")]
    [Range(0.1f, 1f)] public float obstacleCoveragePercentage = 0.25f;
    /// <summary>Hides or shows obstacles.</summary>
	public enum SpawnObstacles
    {
        None,
        ClassicObstacles,
        BuildGrid,
        BuildWall
    }
    public GameObject wallCube;
    public GameObject floorCube;

    public List<NodeObject> tempNodes = new List<NodeObject>();
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
    // TODO: Consider deprecating
    public void GenerateObstacleMap()
    {
        // Find out grid size
        gridArea = (pathManager.gridXSizeHalfLength * 2) * (pathManager.gridZSizeHalfLength * 2);
        int obstacleCount = (int)(gridArea * obstacleCoveragePercentage);
        // Spawn specified amount of obstacles using the total area of the grid. FlagAreas.Grid ensures the entire grid is used.
        for (int i = 0; i < obstacleCount; i++)
        {
            while (true)
            {
                Vector3Int currentObstaclePosition = pathManager.SpawnPointInArea(PathManager.FlagAreas.Grid);
                // Find neighbouring points off current position
                Vector3Int[] currentObstacleNeighbourPositions = new Vector3Int[] {
					// Diagonals - 1st level
					pathManager.FindNodePosition(-1, 1, position: currentObstaclePosition),
                    pathManager.FindNodePosition(1, 1, position: currentObstaclePosition),
                    pathManager.FindNodePosition(1, -1, position: currentObstaclePosition),
                    pathManager.FindNodePosition(-1, -1, position: currentObstaclePosition),
					// Diagonals - 2nd level
					pathManager.FindNodePosition(-2, 2, position: currentObstaclePosition),
                    pathManager.FindNodePosition(2, 2, position: currentObstaclePosition),
                    pathManager.FindNodePosition(2, -2, position: currentObstaclePosition),
                    pathManager.FindNodePosition(-2, -2, position: currentObstaclePosition),
					// Adjacents - Top
					pathManager.FindNodePosition(-1, -2, position: currentObstaclePosition),
                    pathManager.FindNodePosition(-2, -1, position: currentObstaclePosition),
                    pathManager.FindNodePosition(-2, 1, position: currentObstaclePosition),
                    pathManager.FindNodePosition(-1, 2, position: currentObstaclePosition),
					// Adjacents - Bottom
					pathManager.FindNodePosition(1, -2, position: currentObstaclePosition),
                    pathManager.FindNodePosition(2, -1, position: currentObstaclePosition),
                    pathManager.FindNodePosition(2, 1, position: currentObstaclePosition),
                    pathManager.FindNodePosition(1, 2, position: currentObstaclePosition),
                };
                // Iterate through all of the neighbours to make sure no diagonals are found
                // If a diagonal position is filled break the entire loop, otherwise continue
                if (!spawnDiagonals)
                {
                    foreach (Vector3Int neighbour in currentObstacleNeighbourPositions)
                    {
                        if (obstaclePositions.Contains(neighbour))
                            goto EndOfLoop;
                        else
                            continue;
                    }
                }
                // Check to make sure this position isn't already taken
                if (!obstaclePositions.Contains(currentObstaclePosition))
                {
                    obstaclePositions.Add(currentObstaclePosition);
                    Instantiate(obstacleFlag, Vector3.Scale(GlobalStaticVariables.Instance.GlobalScale, currentObstaclePosition), Quaternion.identity);
                    break;
                }
            }
            EndOfLoop:;
        }
    }
    // Working as well. I'm gonna set up triggers for this. - Ty @bubzy-coding
    // Create a list of nodes the size of the whole grid and leave out the path nodes
    public void BuildFullGrid()
    {
        int gridXSizeHalfLength = pathManager.gridXSizeHalfLength;
        int gridZSizeHalfLength = pathManager.gridZSizeHalfLength;
        List<NodeObject> fullGrid = pathManager.pathNodes;
        for (int x = 1; x < gridXSizeHalfLength * 2; x++)
        {
            for (int y = 1; y < gridXSizeHalfLength * 2; y++)
            {
                if (!fullGrid.Any(s => s.position == (new Vector3Int(-gridXSizeHalfLength + x, 0, -gridZSizeHalfLength + y))))
                {
                    fullGrid.Add(new NodeObject(new Vector3Int(-gridXSizeHalfLength + x, 0, -gridZSizeHalfLength + y), 0, 0, 0, false));
                }
            }
        }
        foreach (NodeObject node in fullGrid)
        {
            Instantiate(obstacleFlag, Vector3.Scale(GlobalStaticVariables.Instance.GlobalScale, node.position), Quaternion.identity);
        }
    }
    // This is working real good. So simple. I made some adjustments to make it more readable. - Ty @bubzy-coding
    // Experiment
    public void BuildWall()
    {
        PathManager.GridPoints gridPoints = pathManager.gridPoints;
        List<NodeObject> simplePath = new List<NodeObject>();
        tempNodes = pathManager.pathNodes;
        //add the start and end points as nodes so that they are included in the walls
        tempNodes.Add(new NodeObject(gridPoints.startPointNode, 0, 0, 0, true));
        tempNodes.Add(new NodeObject(gridPoints.endPointNode, 0, 0, 0, true));
        simplePath.AddRange(tempNodes);
        
        //check through positions -1,0 1,0 0,1 0,-1 to see if there is anything present. if not, make a new node in that position and make it unwalkable
        foreach (NodeObject node in simplePath)
        {
            Vector3Int[] checkNeighbours = pathManager.FindNodeNeighbours(node.position, 1);

            foreach (Vector3Int position in checkNeighbours)
            {
                if (!simplePath.Any(nodes => nodes.position == position) && !tempNodes.Any(nodes => nodes.position == position))
                {
                    tempNodes.Add(new NodeObject(position, 0, 0, 0, false));
                }
            }
        }
        
        //visualisation of the list.
        foreach (NodeObject node in tempNodes)
        {
            if (!node.walkable)
                Instantiate(obstacleFlag, Vector3.Scale(GlobalStaticVariables.Instance.GlobalScale, node.position), Quaternion.identity);
        }
        GlobalStaticVariables.Instance.obstacleGenerationComplete = true;
    }
   
    /// <summary>Picks which obstacle type to spawn.</summary>
    public void ObstaclePicker()
    {
        switch (spawnObstacles)
        {
            case SpawnObstacles.None:
                break;
            case SpawnObstacles.ClassicObstacles:
                GenerateObstacleMap();
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