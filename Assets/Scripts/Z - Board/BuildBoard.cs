using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
public class BuildBoard : MonoBehaviour
{

    // Script references
    [Header("Scirpt References")]
    public PathManager pathManager;

    // References
    [Header("Objects")]
    List<NodeObject> wallNodes = new List<NodeObject>();
    List<NodeObject> pathNodes = new List<NodeObject>();
    Vector2Int topLeft = new Vector2Int();
    Vector2Int size = new Vector2Int();
    public GameObject flag;
    public GameObject floorCube;
    public GameObject wallCube;
    public GameObject cornerCube;
    public GameObject pathCube;
    public GameObject pathFinishHole;
    public GameObject boardObjects;

    // Utilities
    [Header("Utilities")]
    public GameObject wallEvisceratorPrefab;
    public GameObject deathCatchPrefab;
    GameObject wallEviscerator;
    [HideInInspector] public GameObject deathCatch;

    /// <summary>This is the main sauce of this source file.</summary>
    public void GetBoardSize()
    {

        boardObjects = new GameObject();
        boardObjects.AddComponent<MeshFilter>();
        boardObjects.AddComponent<MeshRenderer>();
        pathNodes.Clear();
        wallNodes.Clear();
        wallNodes.AddRange(gameObject.GetComponent<ObstacleManager>().obstacleNodes);
        pathNodes.AddRange(gameObject.GetComponent<PathManager>().pathNodes);

        int topLeftX = 0, topLeftY = 0, lowRightX = 0, lowRightY = 0;

        foreach (NodeObject n in wallNodes)
        {
            if (n.position.x < topLeftX) { topLeftX = n.position.x; } else if (n.position.x > lowRightX) { lowRightX = n.position.x; }

            if (n.position.z > topLeftY) { topLeftY = n.position.z; } else if (n.position.z < lowRightY) { lowRightY = n.position.z; }

        }
        float y = pathManager.currentWorldPosition.y;
        Vector3 tpLeft = new Vector3(topLeftX, y, topLeftY);
        Vector3 btRight = new Vector3(lowRightX, y, lowRightY);
        Vector3 midpoint = (tpLeft + btRight) / 2;
        float middleX = topLeftX + (topLeftX - lowRightX) / 2;
        float middleY = topLeftY + (topLeftY - lowRightY) / 2;

        boardObjects.transform.position = midpoint;

        int xSize = Mathf.Abs(topLeftX) + lowRightX;
        int ySize = Mathf.Abs(lowRightY) + topLeftY;
        size = new Vector2Int(xSize, ySize);
        topLeft = new Vector2Int(topLeftX, topLeftY);

        /* -------------------------- Assembly happens here ------------------------- */
        FillGround();
        CreateDeathCatch();
        GroupObjects("floorTile");
        GroupObjects("wallTile");
        MoveWallEviscerator();

        // Store original transform
        pathManager.gridPoints.originalBoardPosition = boardObjects.transform.position;
        pathManager.gridPoints.originalBoardRotation = boardObjects.transform.rotation;
    }

    /// <summary>This creates the floor for the board.</summary>
    void FillGround()
    {
        pathNodes.Reverse();

        // Get reference to path manager
        Vector3Int endPoint = gameObject.GetComponent<PathManager>().gridPoints.endPointNode;
        Vector3Int[] endPointClearance = pathManager.FindClearanceNodes(endPoint);

        // Build out floor
        foreach (NodeObject n in pathNodes)
        {

            // Don't build on the finishing hole
            if (n.position == endPoint)
                Instantiate(pathFinishHole, endPoint + pathFinishHole.transform.position, pathFinishHole.transform.rotation);
            else if (n.position != endPointClearance[0] && n.position != endPointClearance[1] && n.position != endPointClearance[2])
                Instantiate(pathCube, n.position - new Vector3(0, pathCube.transform.localScale.y, 0), pathCube.transform.rotation);
        }

    }

    /// <summary>Simply calculates the center of the board using the start and end points.</summary>
    Vector3 FindBoardCenter()
    {
        Vector3Int startPoint = pathManager.gridPoints.startPointNode;
        Vector3Int endPoint = pathManager.gridPoints.endPointNode;
        return ((Vector3)(endPoint - startPoint) * 0.5f) + startPoint;
    }

    /// <summary>This adds on a collider that destroys exploded wall tiles upon exit.</summary>
    void MoveWallEviscerator()
    {
        // Calculate board center
        Vector3 boardCenter = FindBoardCenter();

        // Spawn wall
        if (wallEviscerator == null)
            wallEviscerator = Instantiate(wallEvisceratorPrefab, boardCenter, Quaternion.identity);
        else
            wallEviscerator.transform.position = boardCenter;

        // Calculate distance between center of board and furthest point (start/end)
        float furthestPoint = (pathManager.gridPoints.endPointNode - pathManager.gridPoints.startPointNode).sqrMagnitude;

        // Adjust radius
        wallEviscerator.GetComponent<SphereCollider>().radius = furthestPoint + 100f;
    }

    /// <summary>This method adds the death catch under the board, which detects if the player died form falling off the board.</summary>
    void CreateDeathCatch()
    {
        // Find board center
        Vector3 boardCenter = FindBoardCenter();

        // Spawn catch if neccessary
        deathCatch = Instantiate(deathCatchPrefab, boardCenter, Quaternion.identity);

        // Set size of death catch
        BoxCollider collider = deathCatch.GetComponent<BoxCollider>();
        float desiredSize = pathManager.desiredPathLength * 3f + 1000f;
        collider.size = new Vector3(desiredSize, collider.size.y, desiredSize);
    }

    /// <summary>This groups all the wall tiles together into a wallTilesGroup object</summary>
    void GroupObjects(string tag)
    {
        GameObject[] placeholdExposedReference = GameObject.FindGameObjectsWithTag(tag);
        boardObjects.name = tag + "s Group";
        boardObjects.tag = "boardObjects";
        foreach (GameObject go in placeholdExposedReference)
        {
            go.transform.parent = boardObjects.transform;
        }
    }

}
