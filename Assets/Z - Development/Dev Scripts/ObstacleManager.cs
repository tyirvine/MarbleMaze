using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ObstacleManager : MonoBehaviour {
	
	public List<NodeObject> obstacleNodes = new List<NodeObject>();
	// This is an inspector setting for setting which obstacle type to spawn
	
	// Link up to the path manager to grab the grid sizing.
	[Header("Script References")]
	public PathManager pathManager;
	public List<Vector3Int> obstaclePositions = new List<Vector3Int>();

	/// <summary>This builds a wall of obstacle nodes around the path.</summary>
	public void BuildWall() {
		obstacleNodes.Clear();
		
		foreach (NodeObject pathNode in pathManager.pathNodes) {
			
		Vector3Int[] checkNeighboursInitial = pathManager.FindNodeNeighbours(pathNode.position, 0);
			
			foreach (Vector3Int position in checkNeighboursInitial) {
				
				if (pathManager.pathNodes.All(node => node.position != position) && !obstacleNodes.Any(node => node.position == position)) 
					{
					obstacleNodes.Add(new NodeObject(position));
				}
				
			}
		}

	}
}