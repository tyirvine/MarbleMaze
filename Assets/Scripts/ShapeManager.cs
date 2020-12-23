using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class ShapeTemplate
{
    [Header("Shape Management")]
    [Tooltip("Insert the premade shape here, in the default orientation")]
    public GameObject shape;
    public GameObject model;
    public Vector3Int currentPosition;
    public int unitCount; //how many squares to make this object?
    public int allowedGaps = 0; //how many gaps are allowed? (THIS WILL AFFECT LEVEL TRAVERSAL AS IT DOESNT CARE ABOUT A PATH CURRENTLY!!!!!!!!)
    public bool includeInBuild = true;
}


public class ShapeManager : MonoBehaviour
{
    public ShapeTemplate[] shapes;
    bool completedChecks = false;
    List<NodeObject> pathNodes = new List<NodeObject>();
    List<Vector3Int> obstacles;
    public GameObject placedFlag;
    private void Update() { if (!completedChecks && GlobalStaticVariables.Instance.obstacleGenerationComplete) { CheckShapes();  } }

    bool checkUnitAgainstList(Vector3 position, NodeObject node)
    {
        
        if (pathNodes.Any(nodes => nodes.position == position && !nodes.walkable))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void CheckShapes()
    {
        completedChecks = true;
        pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
        List<NodeObject> placedNodes = new List<NodeObject>();

        int shapesMade = 0;
        int count = 0;
        foreach (ShapeTemplate shape in shapes)
        {
            if (shape.includeInBuild)
            {
                int obstacleCount = pathNodes.Count;
                Debug.Log("Obstacle Count : " + obstacleCount);
                for (int i = 0; i < obstacleCount; i++)
                {


                    if (!pathNodes[i].walkable && !placedNodes.Any(nodes => nodes.position == pathNodes[i].position))//if the path node is not a wall, then skip it
                    {
                        //move the shape to the new position in the grid
                        shape.shape.transform.position = pathNodes[i].position;

                        //check each child object (unit) in the shape   
                        bool killChild = false;
                        int checkme = 0;
                        foreach (Transform child in shape.shape.transform)
                        {
                            if (!killChild)
                            {
                                checkme++;

                                //    Debug.Log(shape.shape.transform.position + "<parent position " + child.position + " <Child Position " + pathNodes[i].position + "<pathnodes Position " + child.tag );
                                //does the list contain the position of the child? if it does mark it on another list as a constructable object
                                if (checkUnitAgainstList(child.position, pathNodes[i]) && child.tag == ("shapeInvalid"))
                                {
                                    Debug.Log("invalid shape at : " + shape.shape.transform.position + "<Parent Position>" + child.position + " <Child Position> " + pathNodes[i].position + "<pathnodes Position");
                                    killChild = true;
                                }
                                else if (checkUnitAgainstList(child.position, pathNodes[i]) && child.tag != ("shapeInvalid"))
                                {

                                    count++;
                                    //   Debug.Log("COUNT " + count);
                                }
                                //have we got as many positive hits from the object list as the reference number of units in this shape
                                if (count == shape.unitCount - shape.allowedGaps && !killChild && checkme == shape.shape.transform.childCount)
                                {
                                    //add all of the child positions to the list of confirmed shapes
                                    //NOTE : this will have to be more detailed later, perhaps store only the parent position and instantiate a model of the shape in its place,
                                    //or different lists for different shapes. anyway the position can be stored in multiple ways so we can work it out.
                                    Instantiate(shape.model, shape.shape.transform.position, shape.model.transform.rotation);
                                    shapesMade++;
                                    foreach (Transform childPos in shape.shape.transform)
                                    {
                                        // Instantiate(GetComponent<ObstacleManager>().wallCube, childPos.position + new Vector3(0, -1, 0), Quaternion.identity);
                                        if (childPos.tag != "shapeInvalid")
                                        {
                                            placedNodes.Add(new NodeObject(Vector3Int.FloorToInt(childPos.position), 0, 0, 0, false));
                                            //  pathNodes.RemoveAll(nodes => nodes.position == child.position);
                                        }
                                        // obstacleCount--;

                                    }

                                }
                            }
                        }
                    }
                    count = 0;
                }
            }
        }
        Debug.Log("Shapes Made : " + shapesMade);
        foreach(NodeObject node in placedNodes)
        {
            Instantiate(placedFlag, node.position, Quaternion.identity);
        }
    }
}


/*void checkShapes()
{        
    obstacles = gameObject.GetComponent<ObstacleManager>().obstaclePositions;
    //this function currently only checks obstacles, its possible we could add the path too in the preceding lines
    bool checkUnitAgainstList(Vector3 position) { if (obstacles.Contains(Vector3Int.FloorToInt(position))) { return true; } else { return false; } }
    int count = 0;

    foreach (ShapeTemplate shape in shapes)
    {
        int obstacleCount = obstacles.Count;
        for (int i = 0; i < obstacleCount; i++)
        {
            //move the shape to the new position in the grid
            shape.shape.transform.position = new Vector3(obstacles[i].x, 0, obstacles[i].z);
            //check each child object (unit) in the shape              
            foreach (Transform child in shape.shape.transform)
            {
                //does the list contain the position of the child? if it does mark it on another list as a constructable object
                if (checkUnitAgainstList(child.position))
                {
                    count++;
                    //have we got as many positive hits from the object list as the reference number of units in this shape
                    if (count == shape.unitCount - shape.allowedGaps)
                    {
                        //add all of the child positions to the list of confirmed shapes
                        //NOTE : this will have to be more detailed later, perhaps store only the parent position and instantiate a model of the shape in its place,
                        //or different lists for different shapes. anyway the position can be stored in multiple ways so we can work it out.
                        Instantiate(shape.model, shape.shape.transform.position, Quaternion.identity);
                        foreach (Transform childPos in shape.shape.transform)
                        {
                            obstacles.Remove(Vector3Int.FloorToInt(childPos.position));
                            obstacleCount--;
                        }
                    }
                }
            }
            count = 0;
        }
    }
}*/