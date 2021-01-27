using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ObstacleManager : MonoBehaviour
{
    // References
    public GameObject obstacleFlag;

    // This is an inspector setting for setting which obstacle type to spawn
    public List<NodeObject> obstacleNodes = new List<NodeObject>();

    // Link up to the path manager to grab the grid sizing.
    [Header("Script References")]
    public PathManager pathManager;
    public List<Vector3Int> obstaclePositions = new List<Vector3Int>();

    /// <summary>Builds an obstacle flag if it's neccessary.</summary>
    public void SpawnObstacleFlag(Vector3Int currentObstaclePosition)
    {
        if (!pathManager.disablePathFlags) Instantiate(obstacleFlag, Vector3.Scale(GlobalStaticVariables.Instance.GlobalScale, currentObstaclePosition), Quaternion.identity);
    }

    /// <summary>This builds a wall of obstacle nodes around the path.</summary>
    public void BuildWall()
    {
        obstacleNodes.Clear();

        foreach (NodeObject pathNode in pathManager.pathNodes)
        {

            Vector3Int[] checkNeighboursInitial = pathManager.FindNodeNeighbours(pathNode.position, 0);

            foreach (Vector3Int position in checkNeighboursInitial)
            {

                if (pathManager.pathNodes.All(node => node.position != position) && !obstacleNodes.Any(node => node.position == position))
                {
                    obstacleNodes.Add(new NodeObject(position));
                }

            }
        }

    }
}