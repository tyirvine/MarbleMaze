
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;

[Serializable]
public class ShapeTemplate
{
    [Header("Shape Management")]
    [Tooltip("Insert the premade shape here, in the default orientation")]
    public GameObject rule;
    public GameObject model;
    public int unitCount;               //how many Units make this object
    public bool includeInBuild = true;
    public bool rotate = true;
    public int chanceToSpawn = 1000;
    public bool isHazard = false;
}

///create a class to store each child point in a shape, this can be done from the child transform, but this method is a little tidier
///as it doesnt rely on checking a transform.tag each time. 
public class ShapePoints
{
    public Vector3Int position;
    public bool invalid;
    public int mode; //this can be 1, 2 or 3, based on the "tag" of the object, shapePlace, shapeInvalid, shapeDontPlace
}

public class ShapeManager : MonoBehaviour
{

    // Shapes
    [Header("Shapes")]
    public ShapeTemplate[] shapes;

    // Hazards
    [Header("Hazards")]
    public ShapeTemplate hazardBumper;
    public ShapeTemplate hazardSpike;
    public ShapeTemplate hazardLandmine;

    // Settings
    [Header("Draw Flags of obstacle positions")]
    public bool spawnFlags = false;
    public GameObject obstacleFlag;
    // public float difficulty = 1f;

    // Program
    int rotation = 90;
    public void CheckShapes()
    {
        /* ------------------------------ Hazard Shapes ----------------------------- */
        // Keep in mind these need to be removed at the end
        List<ShapeTemplate> shapesAsList = shapes.ToList();
        // shapesAsList.RemoveAll(shape => shape.isHazard);
        shapesAsList.Add(hazardBumper);
        shapesAsList.Add(hazardSpike);
        shapesAsList.Add(hazardLandmine);
        shapes = shapesAsList.ToArray();

        // This simply disables the method from firing
        List<NodeObject> obstacleNodes = new List<NodeObject>();
        obstacleNodes.AddRange(gameObject.GetComponent<ObstacleManager>().obstacleNodes);
        List<Vector3Int> placedPositions = new List<Vector3Int>();
        List<Vector3Int> obstaclePositions = new List<Vector3Int>();
        List<ShapePoints> shapePoints = new List<ShapePoints>();
        List<NodeObject> _walkNodes = new List<NodeObject>();
        _walkNodes.AddRange(gameObject.GetComponent<PathManager>().pathShapeNodes);
        List<Vector3> walkNodes = new List<Vector3>();

        // assign the unwalkable pathNodes to nodePositions and the walkable pathNodes to pathPositions
        for (int i = 0; i < obstacleNodes.Count; i++)
        {
            if (!obstacleNodes[i].walkable)
            {
                obstaclePositions.Add(Vector3Int.RoundToInt(obstacleNodes[i].position));
                if (spawnFlags)
                {
                    Instantiate(obstacleFlag, obstacleNodes[i].position, Quaternion.identity);
                }
            }

        }
        foreach (NodeObject node in _walkNodes)
        {
            walkNodes.Add(node.position);
        }

        // Debug.Log("WALKABLE  : " + walkNodes.Count);
        //sort the array by shape size, this ensures that the largest objects are checked first and does not fill the whole grid with the smallest shape(1x1)
        Array.Sort(shapes, delegate (ShapeTemplate x, ShapeTemplate y) { return x.unitCount.CompareTo(y.unitCount); });
        Array.Reverse(shapes);

        foreach (ShapeTemplate currentShape in shapes)
        {

            if (currentShape.includeInBuild)
            {

                GameObject tempRule = Instantiate(currentShape.rule);
                foreach (Vector3Int nodePosition in obstaclePositions)
                {
                    if (!currentShape.isHazard || SpawnObject(currentShape.chanceToSpawn))
                    {
                        if (!placedPositions.Contains(nodePosition))
                        {
                            int rotation = 1;
                            if (currentShape.rotate)
                            {
                                rotation = 4;
                            }
                            for (int i = 0; i < rotation; i++)
                            {
                                shapePoints.Clear();
                                tempRule.transform.position = nodePosition;
                                tempRule.transform.Rotate(0, i * 90, 0);

                                foreach (Transform childObject in tempRule.transform)
                                {
                                    ShapePoints tempShape = new ShapePoints();
                                    tempShape.position = Vector3Int.RoundToInt(childObject.transform.position);
                                    switch (childObject.tag)
                                    {
                                        case "shapeValid": tempShape.mode = 1; break;    //this is a valid placement point for the object, it will add the position to the list
                                        case "shapeDontPlace": tempShape.mode = 2; break;    //this is a valid check but it will not add the position to the list, leaving the node available for further checks
                                        case "shapeInvalid": tempShape.mode = 3; break;    //this is an invalid placement check. if there is anything at this point then fail the shape check
                                        case "shapeNoPath": tempShape.mode = 4; break;    //this is an invalid placement check IF there is a path present at its location, it will fail the shape check
                                        case "invalidIgnore": tempShape.mode = 5; break;    //this is an invalid placement but ignore it (experimental 20/1/21)
                                        case "shapePathRequired": tempShape.mode = 6; break; // this is a valid placement check, it checks to see if there is a pathnode present at this point
                                        default: tempShape.mode = 3; break;
                                    }
                                    shapePoints.Add(tempShape);
                                }

                                int count = 0;
                                int currentChildCheck = 0;
                                int childCount = tempRule.transform.childCount;
                                int failPoint = childCount - currentShape.unitCount;
                                //Debug.Log("failpoint : " + failPoint + " Child Count : " + childCount + " unitCount : " + currentShape.unitCount);
                                foreach (ShapePoints checkPosition in shapePoints)
                                {
                                    currentChildCheck++;

                                    if (checkPosition.mode == 6 && walkNodes.Contains(checkPosition.position))
                                    {
                                        count++;
                                    }
                                    if (checkPosition.mode == 5 && obstaclePositions.Contains(checkPosition.position))
                                    {
                                        count++;
                                    }
                                    else if (checkPosition.mode == 4 && !walkNodes.Contains(checkPosition.position))
                                    {
                                        count++;
                                        // count = -200; //silly value, probably a better way to ensure failure
                                        // break;
                                    }
                                    else if (checkPosition.mode == 3 && obstaclePositions.Contains(checkPosition.position))
                                    {
                                        count = -200; //silly value, probably a better way to ensure failure
                                        break;
                                    }
                                    else if (checkPosition.mode == 1 || checkPosition.mode == 2)
                                    {
                                        if (obstaclePositions.Contains(checkPosition.position) && !placedPositions.Contains(checkPosition.position))
                                            count++;
                                    }
                                    if (currentChildCheck > failPoint + 1 && count <= 1)
                                    {
                                        // Debug.Log("failed at " + currentChildCheck + " and count : " + count);
                                        break;
                                    }
                                }

                                if (count == currentShape.unitCount) //have we got enough positive hits to build this object? if so, make the model and add the points to the placedPositions list so they are unavailable
                                {
                                    // TODO: Potentially remove - Testing to see if this adds the shape's current rotation properly
                                    tempRule.transform.rotation *= currentShape.model.transform.rotation;
                                    tempRule.transform.position += currentShape.model.transform.position;
                                    // Spawns shape
                                    Instantiate(currentShape.model, tempRule.transform.position, tempRule.transform.rotation);
                                    foreach (ShapePoints placePos in shapePoints)
                                    {
                                        if (placePos.mode == 1)
                                        {
                                            placedPositions.Add(placePos.position);
                                        }
                                    }
                                }
                            }
                            Destroy(tempRule); // this stops the rule being visible at the end of the object generation
                        }
                    }//if currentshape include in build
                }
            }

        }

        // Remove all hazards from shapes
        shapesAsList = shapes.ToList();
        shapesAsList.RemoveAll(shape => shape.isHazard);
        shapes = shapesAsList.ToArray();
    }

    public bool SpawnObject(int pct)
    {
        int rnd = UnityEngine.Random.Range(1, 1000);
        // rnd *= difficulty;
        // if (rnd <= pct * difficulty)
        if (rnd <= pct)
            return true;
        else
            return false;

    }
}