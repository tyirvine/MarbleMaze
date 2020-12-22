using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour {
    // Reference to the obstacle manager for linking up the obstacle position list and generating obstacle maps
    public ObstacleManager obstacleManager;

    // These are half lengths so they can be used as a product of a (1/2) division
    [Header("Grid Size")]
    [Range(5, 100)] public int gridXSizeHalfLength = 10;
    [Range(5, 100)] public int gridZSizeHalfLength = 10;

    // Scaling for the placement of objects on the grid
    Vector3 gridScale;

    /// <summary>This dictates how much of the grid area should be start node spawnable area.</summary>
    [Range(0f, 0.2f)] public float startAreaPercentage = 0.1f;

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

    // Decides how long the path itself should be, measured in integral units.
    [Header("Path Settings")]
    [Range(0.1f, 1.0f)] public float desiredPathLengthPercentage = 0.1f;
    /// <summary>Creates a randomized path if enabled.</summary>
    public bool isPathWacky = false;

    /// <summary>Use this to determine if a path has been succesfully generated or not.</summary>
    [HideInInspector] public bool didPathGenerate;

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
    public Vector3Int[] FindNodeNeighbours(Vector3Int position) {
        return new Vector3Int[] {
					// Non-diagonals
					FindNodePosition(-1, 0, position),
                    FindNodePosition(0, 1, position),
                    FindNodePosition(1, 0, position),
                    FindNodePosition(0, -1, position)
        };
    }

    // =========================================
    // Phase 1: Construct the grid
    // =========================================

    /// <summary>Ensures anything that needs to be reset gets reset.</summary>
    void Initialize() {
        gridPoints = new GridPoints { placedPoints = new List<Vector3Int>() };
        didPathGenerate = false;

        // Obstacle manager
        obstacleManager.obstaclePositions = new List<Vector3Int>();
        obstacleManager.gridArea = 0;
    }

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
        // Find the origin grid position by inverting the gridX and gridZ lengths
        Vector3 originGridPosition = new Vector3Int(-gridXSizeHalfLength, parentYPosition, -gridZSizeHalfLength);

        if (GlobalStaticVariables.Instance.debugMode) {
            Instantiate(originFlag, Vector3.Scale(gridScale, originGridPosition), Quaternion.identity);
        }

        // Simplifies grid position definitions, parent
        Vector3Int ReturnGridPoint(int x, int z) => new Vector3Int(x, parentYPosition, z);

        // Define grid positions
        gridPoints.bottomLeft = ReturnGridPoint(-gridXSizeHalfLength, -gridZSizeHalfLength);
        gridPoints.topLeft = ReturnGridPoint(-gridXSizeHalfLength, gridZSizeHalfLength);
        gridPoints.bottomRight = ReturnGridPoint(gridXSizeHalfLength, -gridZSizeHalfLength);
        gridPoints.topRight = ReturnGridPoint(gridXSizeHalfLength, gridZSizeHalfLength);

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

    // Node object to keep track of path cost
    public class NodeObject {
        public Vector3Int position;
        /// <summary>Distance from the starting node.</summary>
        public int gCost;
        /// <summary>Distance from the end node.</summary>
        public int hCost;
        /// <summary>G cost + H cost.</summary>
        public int fCost { get => gCost + hCost; }

        /// <summary>This is the parent node this node is linked to. Use this to back trace a path from the end to the start.</summary>
        public NodeObject parent;

        // Initializer
        public NodeObject(Vector3Int position, int gCost, int hCost, int fCost) {
            this.position = position;
            this.gCost = gCost;
            this.hCost = hCost;
        }
    }

    /// <summary>Check to see if a position is within the grid bounds or not.</summary>
    public bool CheckIfInGridBounds(Vector3Int position) {
        GridPoints grid = gridPoints;
        if ((position.z < grid.topLeft.z && position.z > grid.bottomRight.z) && (position.x < grid.bottomRight.x && position.x > grid.topLeft.x))
            return true;
        else
            return false;
    }

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
                Vector3Int[] possibleSpawnNeighbours = new Vector3Int[] {
					// Non-diagonals
					FindNodePosition(-1, 0, position: possibleSpawn),
                    FindNodePosition(0, 1, position: possibleSpawn),
                    FindNodePosition(1, 0, position: possibleSpawn),
                    FindNodePosition(0, -1, position: possibleSpawn),
					// Non-diagonals - 2nd Level
					FindNodePosition(-2, 0, position: possibleSpawn),
                    FindNodePosition(0, 2, position: possibleSpawn),
                    FindNodePosition(2, 0, position: possibleSpawn),
                    FindNodePosition(0, -2, position: possibleSpawn),
					// Diagonals
					FindNodePosition(-1, 1, position: possibleSpawn),
                    FindNodePosition(1, 1, position: possibleSpawn),
                    FindNodePosition(1, -1, position: possibleSpawn),
                    FindNodePosition(-1, -1, position: possibleSpawn),
					// Diagonals - 2nd Level
					FindNodePosition(-2, 2, position: possibleSpawn),
                    FindNodePosition(2, 2, position: possibleSpawn),
                    FindNodePosition(2, -2, position: possibleSpawn),
                    FindNodePosition(-2, -2, position: possibleSpawn),
                };

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

    /// <summary>This handles the creation of a path from the start point to the end point!</summary>
    void GeneratePath() {
        // Spawn start and end points
        SpawnStartOrEnd(FlagAreas.Start, startFlag);
        SpawnStartOrEnd(FlagAreas.End, endFlag);

        /// For an explination on what these node lists mean please visit ⤵
        /// https://www.notion.so/scriptobit/Environment-Path-Generation-a5304e8f37474efa98809a03f0e26074
        List<NodeObject> openNodes = new List<NodeObject>();
        List<NodeObject> closedNodes = new List<NodeObject>();
        List<NodeObject> pathNodes = new List<NodeObject>();
        List<NodeObject> clearanceNodes = new List<NodeObject>();

        // Add the start node to the open points list
        openNodes.Add(new NodeObject(gridPoints.startPointNode, 0, 0, 0));

        // This object contains the current node being investigated
        NodeObject currentNode;

        /// <summary>Finds all the clearance nodes for the provided position. Top, top right, and right side.</summary>
        Vector3Int[] FindClearanceNodes(Vector3Int position) {
            return new Vector3Int[] {
                FindNodePosition(0, 1, position),
                FindNodePosition(1, 1, position),
                FindNodePosition(1, 0, position)
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
            // Reverse the list because we started tracing from the end, and calculate the path's length
            pathNodes.Reverse();
            // Instantiate desired object
            foreach (NodeObject node in pathNodes)
                Instantiate(pathFlag, Vector3.Scale(gridScale, node.position), Quaternion.identity);
        }

        /// <summary>This checks to see if the point collides with any non-pathable positions.</summary>
        bool IsPathable(NodeObject node) {
            // Grab clearance neighbours to provided position
            Vector3Int[] positionClearanceNeighbours = FindClearanceNodes(node.position);
            bool isPathable = false;

            // Grab corner positions
            CornerNode[] pathNodeCorners = new CornerNode[] {
                new CornerNode(FindNodePosition(-1, -1, node.position), Corner.BottomLeft),
                new CornerNode(FindNodePosition(-1, 1, positionClearanceNeighbours[0]), Corner.TopLeft),
                new CornerNode(FindNodePosition(1, 1, positionClearanceNeighbours[1]), Corner.TopRight),
                new CornerNode(FindNodePosition(1, -1, positionClearanceNeighbours[2]), Corner.BottomRight)
            };

            // Start by making sure the point is within the grid bounds
            foreach (Vector3Int position in positionClearanceNeighbours)
                if (CheckIfInGridBounds(position))
                    isPathable = true;
                else
                    return false;

            // Checks a corner node's neighbours
            bool ContainsAdjacentNodes(Corner corner, Vector3Int position) {
                Vector3Int adjacentOne = Vector3Int.zero;
                Vector3Int adjacentTwo = Vector3Int.zero;

                // Finds adjacent corner positions depending on corner
                switch (corner) {
                    case Corner.TopLeft:
                        adjacentOne = FindNodePosition(0, -1, position);
                        adjacentTwo = FindNodePosition(1, 0, position);
                        break;
                    case Corner.TopRight:
                        adjacentOne = FindNodePosition(-1, 0, position);
                        adjacentTwo = FindNodePosition(0, -1, position);
                        break;
                    case Corner.BottomLeft:
                        adjacentOne = FindNodePosition(0, 1, position);
                        adjacentTwo = FindNodePosition(1, 0, position);
                        break;
                    case Corner.BottomRight:
                        adjacentOne = FindNodePosition(-1, 0, position);
                        adjacentTwo = FindNodePosition(0, 1, position);
                        break;
                }

                // Checks to see if closed nodes contains either of the adjacent nodes
                if (closedNodes.Any(nodes => nodes.position == adjacentOne) || closedNodes.Any(nodes => nodes.position == adjacentTwo))
                    return true;
                // Otherwise that means we found a diagonal so...
                else
                    return false;
            }

            // Then check each corner to make sure it's not a diagonal pinch
            foreach (CornerNode cornerNode in pathNodeCorners)
                if (closedNodes.Any(nodes => nodes.position == cornerNode.position)) {
                    // If it does contain a node on the corner, then it checks for adjacents
                    if (!ContainsAdjacentNodes(cornerNode.corner, cornerNode.position))
                        return false;
                    else
                        isPathable = true;
                } else
                    isPathable = true;

            return isPathable;
        }

        /// <summary>Grabs the difference in distance of both the x & z points between Node A and Node B.</summary>
        int GetDistance(NodeObject nodeA, NodeObject nodeB) {
            int dstX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
            int dstZ = Mathf.Abs(nodeA.position.z - nodeB.position.z);
            if (!isPathWacky) {
                // Bubzy do you know what's happening here?
                if (dstX > dstZ) return 14 * dstZ + 10 * (dstX - dstZ);
                return 14 * dstX + 10 * (dstZ - dstX);
            }
            // and here?
            if (dstX > dstZ) return Random.Range(1, 30) * dstZ + 10 * (dstX - dstZ);
            return Random.Range(1, 30) * dstX + 10 * (dstZ - dstX);
        }

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

            // Add clearance neighbours as well
            Vector3Int[] clearanceNeighbours = FindClearanceNodes(currentNode.position);
            foreach (Vector3Int position in clearanceNeighbours)
                closedNodes.Add(new NodeObject(position, 0, 0, 0));

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
            new NodeObject(FindNodePosition(-2, 0, currentNode: currentNode), 0, 0, 0),
            new NodeObject(FindNodePosition(0, 2, currentNode: currentNode), 0, 0, 0),
            new NodeObject(FindNodePosition(2, 0, currentNode: currentNode), 0, 0, 0),
            new NodeObject(FindNodePosition(0, -2, currentNode: currentNode), 0, 0, 0)
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
                    node.hCost = GetDistance(node, new NodeObject(gridPoints.endPointNode, 0, 0, 0));
                    node.parent = currentNode;
                    // If the node is not found in the open nodes list then add it
                    if (!openNodes.Any(nodes => nodes.position == node.position))
                        openNodes.Add(node);
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

    // Restart from here
    RestartLoop:;
        // Initialize
        Initialize();
        GameObject[] flags;
        GameObject[] obstacleFlags;
        // Destroy all flags first
        flags = GameObject.FindGameObjectsWithTag("Flag");
        foreach (GameObject flag in flags) GameObject.Destroy(flag);
        obstacleFlags = GameObject.FindGameObjectsWithTag("pathObstacle");
        foreach (GameObject flag in obstacleFlags) GameObject.Destroy(flag);

        // Build the grid and spawn the obstacles
        ConstructGrid();
        obstacleManager.GenerateObstacleMap();

        // This catch is looking for a `No sequence` error that can occur when the path can't go from start to finish
        try {
            // Generates the entire path
            GeneratePath();
        } catch {
            Debug.LogWarning("Error caught - Loop Reset");
            errorCaught = true;
            goto RestartLoop;
        }
        // A valid path has been generated!
        if (errorCaught) Debug.Log("Error resolved - Loop Completed!");

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
        GlobalStaticVariables.Instance.obstacleGenerationComplete = true;

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
