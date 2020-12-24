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
    //public Vector3Int currentPosition;
    public int unitCount; //how many squares to make this object?
    //public int allowedGaps = 0; //how many gaps are allowed? (THIS WILL AFFECT LEVEL TRAVERSAL AS IT DOESNT CARE ABOUT A PATH CURRENTLY!!!!!!!!)
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

    bool checkLinq(List<NodeObject> node, Vector3 checkPos)
    {
        return node.Any(nodes => nodes.position == checkPos);
    }
        
   
    void CheckShapes()
    {
        
        completedChecks = true;
        pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
        List<Vector3> placedPositions = new List<Vector3>();
        List<Vector3> nodePositions = new List<Vector3>();
        List<Vector3> pathPositions = new List<Vector3>();

        ///<summary>Check the input Vector 3 against 2 position lists to ensure theres no clash</summary>
        bool CheckForInvalidShape(Vector3 position) 
        {
            if (nodePositions.Contains(position)  || placedPositions.Contains(position))
                            {                return true;            } //the position is in one of the lists, therefore it is not a valid placement position
            else            {                return false;           }
        }

        // assign the unwalkable pathNodes to nodePositions and the walkable pathNodes to pathPositions
        for (int i = 0; i < pathNodes.Count; i++)
        {   if      (!pathNodes[i].walkable)            {                nodePositions.Add(pathNodes[i].position);            }
            else                                        {                pathPositions.Add(pathNodes[i].position);            }        }
        

        foreach(ShapeTemplate currentShape in shapes) // iterate through all the shapes
        {
            //only build the shapes that are selected in the inspector
            int checkedAllChildren = currentShape.shape.transform.childCount;
            if (currentShape.includeInBuild)
            {
                foreach (Vector3 nodePosition in nodePositions)
                {
                    //move the shape through the existing positions and see if the children match the pattern 
                    currentShape.shape.transform.position = nodePosition;
                    int count = 0;

                    if (!placedPositions.Contains(nodePosition))
                    {

                        int currentChildChecks = 0;
                        foreach (Transform child in currentShape.shape.transform)
                        {
                            
                            if (child.tag == "shapeInvalid" && CheckForInvalidShape(child.position))
                            {
                                Debug.Log("Invalid Position");
                                currentChildChecks = 0;
                                count = 0;
                                break;                                
                            }

                            if (nodePositions.Contains(child.position) && !placedPositions.Contains(child.position)&&child.tag!="shapeInvalid")
                            {                                
                                count++;
                            }
                            currentChildChecks++;
                        }
                        if (count == currentShape.unitCount&&currentChildChecks == checkedAllChildren)
                        {
                            Instantiate(currentShape.model, currentShape.shape.transform.position, currentShape.model.transform.rotation);
                            foreach (Transform child in currentShape.shape.transform)
                            {
                                if (child.tag != "shapeInvalid")
                                    placedPositions.Add(child.position);
                            }
                            count = 0;
                        }
                    }
                }
            }
        }
        

    }
}
/*
void CheckShapes()
{
    completedChecks = true;
    pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
    List<NodeObject> placedPositions = new List<NodeObject>(); //store already placed obstacles so that new ones dont overlap

    int count = 0;
    foreach (ShapeTemplate shape in shapes)
    {
        if (shape.includeInBuild)
        {
            int obstacleCount = pathNodes.Count;
            Debug.Log("Obstacle Count : " + obstacleCount);
            for (int i = 0; i < obstacleCount; i++)
            {


                if (!pathNodes[i].walkable && !placedPositions.Any(nodes => nodes.position == pathNodes[i].position))//if the path node is not a wall, then skip it
                {
                    //move the shape to the new position in the grid
                    shape.shape.transform.position = pathNodes[i].position;

                    int checkme = 0;
                    foreach (Transform child in shape.shape.transform)
                    {
                        checkme++;

                        //    Debug.Log(shape.shape.transform.position + "<parent position " + child.position + " <Child Position " + pathNodes[i].position + "<pathnodes Position " + child.tag );
                        //does the list contain the position of the child? if it does mark it on another list as a constructable object
                        //if (checkUnitAgainstList(child.position, pathNodes[i]) && child.tag == ("shapeInvalid"))
                        //{
                        //    Debug.Log("invalid shape at : " + shape.shape.transform.position + "<Parent Position>" + child.position + " <Child Position> " + pathNodes[i].position + "<pathnodes Position");
                        //}
                        if (checkUnitAgainstList(child.position, pathNodes[i]) && child.tag != ("shapeInvalid") && !placedPositions.Any(nodes => nodes.position == child.position))
                        {
                            count++;
                        }
                        //have we got as many positive hits from the object list as the reference number of units in this shape
                        if (count == shape.unitCount - shape.allowedGaps && checkme == shape.shape.transform.childCount)
                        {
                            //add all of the child positions to the list of confirmed shapes
                            //NOTE : this will have to be more detailed later, perhaps store only the parent position and instantiate a model of the shape in its place,
                            //or different lists for different shapes. anyway the position can be stored in multiple ways so we can work it out.
                            Instantiate(shape.model, shape.shape.transform.position, shape.model.transform.rotation);

                            foreach (Transform childPos in shape.shape.transform)
                            {
                                // Instantiate(GetComponent<ObstacleManager>().wallCube, childPos.position + new Vector3(0, -1, 0), Quaternion.identity);
                                if (childPos.tag != "shapeInvalid")
                                {
                                    placedPositions.Add(new NodeObject(Vector3Int.FloorToInt(childPos.position), 0, 0, 0, false));
                                    pathNodes.RemoveAll(nodes => nodes.position == child.position);
                                    obstacleCount--;
                                }


                            }

                        }

                    }
                }
                count = 0;
            }
        }
    }

    foreach (NodeObject node in placedPositions)
    {
        Instantiate(placedFlag, node.position, Quaternion.identity);
    }
}
}
*/
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
