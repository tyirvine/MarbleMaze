﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
public class BuildBoard : MonoBehaviour
{
    List<NodeObject> pathNodes = new List<NodeObject>();
    Vector2Int topLeft = new Vector2Int();
    Vector2Int size = new Vector2Int();
    public GameObject flag;
    public GameObject floorCube;
    public GameObject wallCube;
    public GameObject cornerCube;
    public GameObject pathCube;
    public GameObject boardObjects;


    public GameObject Marble;
    void Awake()
    {
        boardObjects = new GameObject();
        boardObjects.AddComponent<MeshFilter>();
        boardObjects.AddComponent<MeshRenderer>();
        //GetBoardSize();
    }

    public void GetBoardSize()
    {
        pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
        
        int topLeftX = 0, topLeftY = 0, lowRightX = 0, lowRightY = 0;
        
        foreach (NodeObject n in pathNodes)
        {
         if (n.position.x < topLeftX)            {                topLeftX = n.position.x;            }
         else if (n.position.x > lowRightX)      {                lowRightX = n.position.x;           }

         if(n.position.z > topLeftY)             {                topLeftY = n.position.z;            }
         else if(n.position.z < lowRightY)       {                lowRightY = n.position.z;           }
            
        }

        int xSize = Mathf.Abs(topLeftX) + lowRightX;
        int ySize = Mathf.Abs(lowRightY) + topLeftY;
        size = new Vector2Int(xSize, ySize);
        topLeft = new Vector2Int(topLeftX, topLeftY);

        ///debug to make sure we got the board size right.
        Instantiate(flag, new Vector3(topLeftX, 1, topLeftY), Quaternion.identity);
        Instantiate(flag, new Vector3(lowRightX, 1, lowRightY), Quaternion.identity);
        Debug.Log(topLeftY + " " + lowRightY);
        Debug.Log("Size X : " + xSize + " y: " + ySize);

        ///
        BuildWall();
        FillSpaces();
        GroupObjects("floorTile");
        MakeSingleMesh();
        PlaceMarble();
        GroupObjects("wallTile");
    }

    void PlaceMarble()
    {
        PathManager.GridPoints gridPoints = GetComponent<PathManager>().gridPoints;
        Instantiate(Marble, gridPoints.startPointNode, Quaternion.identity);

    }
    void GroupObjects(string tag)
    {
        GameObject[] placeholdExposedReference = GameObject.FindGameObjectsWithTag(tag);
        
        boardObjects.name = tag + "s Group";
        foreach(GameObject go in placeholdExposedReference)
        {
            go.transform.parent = boardObjects.transform;
        }
    }
    void BuildWall()
    {
        for (int x = topLeft.x-1; x <= topLeft.x + size.x+1; x++)
        {
            for (int y = topLeft.y - size.y-1; y <= topLeft.y+1; y++)
            {
                if( x== topLeft.x-1 && y== topLeft.y+1)
                {
                    Instantiate(cornerCube, new Vector3(x, 0, y), Quaternion.Euler(0, -90, 0));
                    continue;
                }
                if(x==topLeft.x+size.x+1&&y==topLeft.y+1)
                {
                    Instantiate(cornerCube, new Vector3(x, 0, y), Quaternion.Euler(0, 0, 0));
                    continue;
                }
                if(x==topLeft.x-1 && y==topLeft.y-size.y-1)
                {
                    Instantiate(cornerCube, new Vector3(x, 0, y), Quaternion.Euler(0, 180, 0));
                    continue;
                }
                if(x==topLeft.x+size.x+1 && y == topLeft.y - size.y - 1)
                {
                    Instantiate(cornerCube, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0));
                    continue;
                }




                if (x == topLeft.x - 1 || x == topLeft.x + size.x + 1)
                {
                    Instantiate(wallCube, new Vector3(x, 0, y), Quaternion.identity);
                }
                if(y==topLeft.y-size.y - 1 || y == topLeft.y+1)
                {
                    Instantiate(wallCube, new Vector3(x, 0, y), Quaternion.identity);
                }
             
            }
        }
    }


    /// <summary>
    /// we dont have any code references for the blank space, this mmight be useful at some point to generate the mesh ouside the play area as a single object
    /// </summary>
    void FillSpaces()
    {
        List<NodeObject> fullGrid = new List<NodeObject>();
        for(int x = topLeft.x ; x <= topLeft.x + size.x; x++)
        {
            for(int y = topLeft.y-size.y; y<=topLeft.y;y++)
            {
                fullGrid.Add(new NodeObject(new Vector3Int(x, 0, y), 0, 0, 0, false));
            }
        }
        Debug.Log(fullGrid.Count);
        List<NodeObject> tempGrid = new List<NodeObject>();
        List<NodeObject> pathGrid = new List<NodeObject>();
        tempGrid.AddRange(fullGrid);
        foreach (NodeObject n in fullGrid)
        {
            Instantiate(floorCube, new Vector3(n.position.x, -1f, n.position.z), Quaternion.identity);
        }
        foreach (NodeObject n in fullGrid)
        {
            if(pathNodes.Any(node =>node.position == n.position))
            {
                tempGrid.Remove(n);
                pathGrid.Add(n);
            }

        }
        Debug.Log(tempGrid.Count);
        fullGrid.Clear();
        fullGrid.AddRange(tempGrid);

       
      //  foreach(NodeObject n in pathGrid)
        //{
          //  Instantiate(pathCube, new Vector3(n.position.x, -1f, n.position.z), Quaternion.identity);
        //}
        

    }

    void MakeSingleMesh()
    {


        if (GlobalStaticVariables.Instance.renderBoardAsSingleMesh)
        {
            MeshFilter[] meshfilters = boardObjects.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshfilters.Length];
            boardObjects.AddComponent<MeshCollider>().convex = true ;
            int i = 0;
            while (i < meshfilters.Length)
            {
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
    
  

   

    void OnTestKey()
    {
        Debug.Log("WHAT THE HELL THOUGH");
    }
    private void FixedUpdate()
    {
       
    
    }
    // Update is called once per frame


    
}