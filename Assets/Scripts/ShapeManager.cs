using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
}


public class ShapeManager : MonoBehaviour
{
    public ShapeTemplate[] shapes;
    bool completedChecks = false;
    List<Vector3Int> obstacles;

    private void Update()    { if(!completedChecks && GlobalStaticVariables.Instance.obstacleGenerationComplete) { checkShapes(); completedChecks = true; } }

    void checkShapes()
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
    }
   
}

