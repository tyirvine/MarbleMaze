using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour
{

    // Scaling for the placement of objects on the grid
    Vector3 gridScale;

    // Decides how long the path itself should be, measured in integral units.
    [Header("Path Settings")]
    [Range(6, 100)] public int desiredPathLength = 6;

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
    [HideInInspector] public Vector3Int currentWorldPosition;
    /// <summary>Reference to y position of the parent. Used to ensure grid positions are all at the same y position.</summary>
    [HideInInspector] public int parentYPosition { get => currentWorldPosition.y; }

    /// For an explination on what these node lists mean please visit ⤵
    /// https://www.notion.so/scriptobit/Environment-Path-Generation-a5304e8f37474efa98809a03f0e26074
    public List<NodeObject> openNodes = new List<NodeObject>();
    public List<NodeObject> closedNodes = new List<NodeObject>();
    public List<NodeObject> pathNodes = new List<NodeObject>();
    public List<NodeObject> clearanceNodes = new List<NodeObject>();

    /// <summary>This list contains path nodes that are specifically refined for the ShapeManager.</summary>
    public List<NodeObject> pathShapeNodes = new List<NodeObject>();

    /// <summary> Use this object to define grid positions.</summary>
    public struct GridPoints
    {

        // TODO: Finish Death Catch
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

    // For determining whether or not to spawn the start or end point. Helps with readability
    public enum FlagAreas
    {
        Start,
        End,
        Grid
    }

    // Used to determine which corner
    public enum Corner
    {
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
    public Vector3Int FindNodePosition(int xOffset, int zOffset, [Optional] Vector3Int position, NodeObject currentNode = null)
    {
        // Below is a ternary operator, here's a link if you're not familiar → https://bit.ly/39O0q8e
        Vector3Int referencePosition = currentNode == null ? position : currentNode.position;
        return new Vector3Int(referencePosition.x + xOffset, parentYPosition, referencePosition.z + zOffset);
    }

    /// <summary>Simplifies finding the neighbour positions of a node.</summary>
    /// <example>mode 1 is diagonals, mode 2 is non diagonals</example>
    public Vector3Int[] FindNodeNeighbours(Vector3Int position, int mode)
    {
        switch (mode)
        {
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
    void Initialize()
    {
        gridPoints = new GridPoints { placedPoints = new List<Vector3Int>() };
        didPathGenerate = false;

        openNodes = new List<NodeObject>();
        closedNodes = new List<NodeObject>();
        pathNodes = new List<NodeObject>();
        clearanceNodes = new List<NodeObject>();
        pathShapeNodes = new List<NodeObject>();

        // Obstacle manager
        obstacleManager.obstaclePositions = new List<Vector3Int>();

    }

    /// <summary>Finds all the clearance nodes for the provided position. Top, top right, and right side.</summary>
    public Vector3Int[] FindClearanceNodes(Vector3Int position)
    {
        return new Vector3Int[] {
                FindNodePosition(0, 1, position),
                FindNodePosition(1, 1, position),
                FindNodePosition(1, 0, position),
            };
    }

    /// <summary>This back tracks from the current node to find the starting node, making a path.</summary>
    public void RetracePath(NodeObject lastNode)
    {
        NodeObject traceNode = lastNode;

        /// <summary>This links the child node to the parent node by spawning nodes inbetween.</summary>
        void AddPathNodeAtOffset(NodeObject node, String axis, int offset)
        {
            Vector3Int newNodePos;
            Vector3Int newNodePosSecond;
            if (axis == "x")
            {
                newNodePos = new Vector3Int(node.position.x, parentYPosition, node.position.z + offset);
                newNodePosSecond = new Vector3Int(node.position.x, parentYPosition, node.position.z + (offset * 2));
            }
            else
            {
                newNodePos = new Vector3Int(node.position.x + offset, parentYPosition, node.position.z);
                newNodePosSecond = new Vector3Int(node.position.x + (offset * 2), parentYPosition, node.position.z);
            }
            pathNodes.Add(new NodeObject(newNodePos));
            pathNodes.Add(new NodeObject(newNodePosSecond));
        }

        // Link all nodes by creating a new node between the parent and child
        foreach (NodeObject node in closedNodes)
        {
            if (node.parent != null)
            {
                // Check if both nodes are on the x-axis
                if (node.position.x == node.parent.position.x)
                {
                    if (node.position.z > node.parent.position.z)
                    {
                        AddPathNodeAtOffset(node, "x", -1);
                    }
                    else
                    {
                        AddPathNodeAtOffset(node, "x", 1);
                    }
                }
                // Check if both nodes are on the z-axis
                else if (node.position.z == node.parent.position.z)
                {
                    if (node.position.x > node.parent.position.x)
                    {
                        AddPathNodeAtOffset(node, "z", -1);
                    }
                    else
                    {
                        AddPathNodeAtOffset(node, "z", 1);
                    }
                }
            }
            // Add current pathnode
            pathNodes.Add(node);
        }

        // Setup an extra list so the for loop doesn't grow ↴
        NodeObject[] pathNodesClearance = pathNodes.ToArray();
        // Add in clearance nodes to path nodes
        foreach (NodeObject node in pathNodesClearance)
        {
            Vector3Int[] clearancePositions = FindClearanceNodes(node.position);
            foreach (Vector3Int position in clearancePositions)
                if (pathNodes.All(nodes => nodes.position != position))
                    pathNodes.Add(new NodeObject(position, 0, 0, 0, true));
        }

        // Build pathnodes list for ShapeManager
        pathShapeNodes = pathNodes.ToList();
        // Remove clearance nodes from both start/end points
        Vector3Int[] startendPoints = { gridPoints.startPointNode, gridPoints.endPointNode };
        foreach (Vector3Int position in startendPoints)
        {
            Vector3Int[] clearancePositions = FindClearanceNodes(position);
            foreach (Vector3Int clearancePosition in clearancePositions)
                pathShapeNodes.RemoveAll(nodes => nodes.position == clearancePosition);
        }

        // Remove start / ends as well
        pathShapeNodes.RemoveAll(nodes => nodes.position == gridPoints.startPointNode);
        pathShapeNodes.RemoveAll(nodes => nodes.position == gridPoints.endPointNode);

        // Reverse the list because we started tracing from the end, and calculate the path's length
        pathNodes.Reverse();
        pathShapeNodes.Reverse();

        // Instantiate desired object
        if (!disablePathFlags)
        {
            foreach (NodeObject node in pathShapeNodes)
            {
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
    void GeneratePath()
    {
        // Grab current position for starting point
        gridPoints.startPointNode = currentWorldPosition;

        // Add the start node to the open points list
        openNodes.Add(new NodeObject(gridPoints.startPointNode, 0, 0, 0, false));

        // This object contains the current node being investigated
        NodeObject currentNode = new NodeObject(gridPoints.startPointNode);

        // This variable keeps track of how much progress it's made in getting to the desired path length
        int pathLengthProgress = 0;

        // Loop Emergency Break
        int loopEmergencyBrake = 0;
        int loopEmergencyBrakeCap = 5000;

        // This loops until a path is generated from the start node to the end node
        while (loopEmergencyBrake < loopEmergencyBrakeCap)
        {

            if (pathLengthProgress > 0)
            {
                // Add previous node (known as current) to the closed nodes
                closedNodes.Add(currentNode);
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
            if (pathLengthProgress >= desiredPathLength)
            {
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
            foreach (NodeObject node in neighbourNodes)
            {
                // Check to see if the current neighbour node is intersecting with a closed node
                if (closedNodes.Any(nodes => nodes.position == node.position))
                {
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
    public void ConstructPathStack(Vector3Int spawnPosition)
    {
        // A bool switch to see if an error was caught on the try carch
        bool errorCaught = false;
        // Sets up the designated spawn point
        currentWorldPosition = spawnPosition;

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
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floorTile");
        foreach (GameObject wall in walls)
        {
            Destroy(wall);
        }
        foreach (GameObject floor in floors)
        {
            Destroy(floor);

        }

        // This catch is looking for a `No sequence` error that can occur when the path can't go from start to finish
        try
        {
            // Generates the entire path
            GeneratePath();
            if (buildObstacles) obstacleManager.BuildWall();
        }
        catch (Exception e)
        {
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
}
