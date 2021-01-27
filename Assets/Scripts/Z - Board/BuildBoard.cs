using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
public class BuildBoard : MonoBehaviour {

	// Script refs
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

	public GameObject marble;
	void Awake() {

		//GetBoardSize();
	}

	public void GetBoardSize() {
		boardObjects = new GameObject();
		boardObjects.AddComponent<MeshFilter>();
		boardObjects.AddComponent<MeshRenderer>();
		pathNodes.Clear();
		wallNodes.Clear();
		wallNodes.AddRange(gameObject.GetComponent<ObstacleManager>().obstacleNodes);
		pathNodes.AddRange(gameObject.GetComponent<PathManager>().pathNodes);

		int topLeftX = 0, topLeftY = 0, lowRightX = 0, lowRightY = 0;

		foreach (NodeObject n in wallNodes) {
			if (n.position.x < topLeftX) { topLeftX = n.position.x; } else if (n.position.x > lowRightX) { lowRightX = n.position.x; }

			if (n.position.z > topLeftY) { topLeftY = n.position.z; } else if (n.position.z < lowRightY) { lowRightY = n.position.z; }

		}
		Vector3 tpLeft = new Vector3(topLeftX, 0, topLeftY);
		Vector3 btRight = new Vector3(lowRightX, 0, lowRightY);
		Vector3 midpoint = (tpLeft + btRight) / 2;
		float middleX = topLeftX +  (topLeftX - lowRightX) / 2;
		float middleY = topLeftY +  (topLeftY - lowRightY) / 2;

		boardObjects.transform.position = midpoint;
			 
		int xSize = Mathf.Abs(topLeftX) + lowRightX;
		int ySize = Mathf.Abs(lowRightY) + topLeftY;
		size = new Vector2Int(xSize, ySize);
		topLeft = new Vector2Int(topLeftX, topLeftY);

	
		FillGround();
		GroupObjects("floorTile");
		//MakeSingleMesh();
		PlaceMarble();
		GroupObjects("wallTile");
	}

	void FillGround() {
		Debug.Log(pathNodes.Count + "PATHNODES COUNT");
		pathNodes.Reverse();

		// Get reference to path manager
		Vector3Int endPoint = gameObject.GetComponent<PathManager>().gridPoints.endPointNode;
		Vector3Int[] endPointClearance = pathManager.FindClearanceNodes(endPoint);

		// Build out floor
		foreach (NodeObject n in pathNodes) {

			// Don't build on the finishing hole
			if (n.position == endPoint)
				Instantiate(pathFinishHole, endPoint + pathFinishHole.transform.position, pathFinishHole.transform.rotation);
			else if (n.position != endPointClearance[0] && n.position != endPointClearance[1] && n.position != endPointClearance[2])
				Instantiate(pathCube, n.position - new Vector3(0, pathCube.transform.localScale.y, 0), pathCube.transform.rotation);
		}
		foreach (NodeObject n in wallNodes) {
			// Instantiate(wallCube, n.position - new Vector3Int(0, 1, 0), pathCube.transform.rotation);
		}

	}

	void PlaceMarble() {
		PathManager.GridPoints gridPoints = GetComponent<PathManager>().gridPoints;
		Instantiate(marble, gridPoints.endPointNode +  new Vector3Int(0,5,0), Quaternion.identity);

	}
	void GroupObjects(string tag) {
		GameObject[] placeholdExposedReference = GameObject.FindGameObjectsWithTag(tag);

		boardObjects.name = tag + "s Group";
		boardObjects.tag = "boardObjects"
; foreach (GameObject go in placeholdExposedReference) {
			go.transform.parent = boardObjects.transform;
		}
	}
	void BuildWall() {
		for (int x = topLeft.x - 1; x <= topLeft.x + size.x + 1; x++) {
			for (int y = topLeft.y - size.y - 1; y <= topLeft.y + 1; y++) {
				if (x == topLeft.x - 1 && y == topLeft.y + 1) {
					Instantiate(cornerCube, new Vector3(x, 0, y), Quaternion.Euler(0, -90, 0));
					continue;
				}
				if (x == topLeft.x + size.x + 1 && y == topLeft.y + 1) {
					Instantiate(cornerCube, new Vector3(x, 0, y), Quaternion.Euler(0, 0, 0));
					continue;
				}
				if (x == topLeft.x - 1 && y == topLeft.y - size.y - 1) {
					Instantiate(cornerCube, new Vector3(x, 0, y), Quaternion.Euler(0, 180, 0));
					continue;
				}
				if (x == topLeft.x + size.x + 1 && y == topLeft.y - size.y - 1) {
					Instantiate(cornerCube, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0));
					continue;
				}

				if (x == topLeft.x - 1 || x == topLeft.x + size.x + 1) {
					Instantiate(wallCube, new Vector3(x, 0, y), Quaternion.identity);
				}
				if (y == topLeft.y - size.y - 1 || y == topLeft.y + 1) {
					Instantiate(wallCube, new Vector3(x, 0, y), Quaternion.identity);
				}

			}
		}
	}

	/// <summary>
	/// we dont have any code references for the blank space, this mmight be useful at some point to generate the mesh ouside the play area as a single object
	/// </summary>
	void FillSpaces() {
		List<NodeObject> fullGrid = new List<NodeObject>();
		for (int x = topLeft.x; x <= topLeft.x + size.x; x++) {
			for (int y = topLeft.y - size.y; y <= topLeft.y; y++) {
				fullGrid.Add(new NodeObject(new Vector3Int(x, 0, y), 0, 0, 0, false));
			}
		}
		//Debug.Log(fullGrid.Count);
		List<NodeObject> tempGrid = new List<NodeObject>();
		List<NodeObject> pathGrid = new List<NodeObject>();
		tempGrid.AddRange(fullGrid);
		foreach (NodeObject n in tempGrid) {
			//      Instantiate(floorCube, new Vector3(n.position.x, -1f, n.position.z), Quaternion.identity);
		}
		foreach (NodeObject n in fullGrid) {
			if (wallNodes.Any(node => node.position == (n.position))) {
				tempGrid.Remove(n);
				pathGrid.Add(n);
			}

		}
		Debug.Log(tempGrid.Count);
		fullGrid.Clear();
		fullGrid.AddRange(tempGrid);

		foreach (NodeObject n in pathGrid) {
			Instantiate(pathCube, new Vector3(n.position.x, -1f, n.position.z), Quaternion.identity);
		}

	}

	void MakeSingleMesh() {

		if (GlobalStaticVariables.Instance.renderBoardAsSingleMesh) {
			MeshFilter[] meshfilters = boardObjects.GetComponentsInChildren<MeshFilter>();
			CombineInstance[] combine = new CombineInstance[meshfilters.Length];
			boardObjects.AddComponent<MeshCollider>().convex = true;
			int i = 0;
			while (i < meshfilters.Length) {
				combine[i].mesh = meshfilters[i].sharedMesh;
				combine[i].transform = meshfilters[i].transform.localToWorldMatrix;
				meshfilters[i].gameObject.SetActive(false);
				i++;
			}
			boardObjects.transform.GetComponent<MeshFilter>().mesh = new Mesh();
			boardObjects.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
			boardObjects.transform.gameObject.SetActive(true);
			boardObjects.transform.GetComponent<MeshCollider>().sharedMesh = boardObjects.transform.GetComponent<MeshFilter>().mesh;
			// transform.GetComponent<MeshCollider>().material = slippyMaterial;
		}
		boardObjects.AddComponent<Rigidbody>();
		Rigidbody boardBody = boardObjects.GetComponent<Rigidbody>();
		boardBody.useGravity = false;
		boardBody.isKinematic = true;
	}

	void OnTestKey() {
		Debug.Log("WHAT THE HELL THOUGH");
	}
	private void FixedUpdate() {

	}
	// Update is called once per frame

}
