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
    public GameObject shape;
    public GameObject model;
    public int unitCount;               //how many Units make this object
    public bool includeInBuild = true;
    
}

///create a class to store each child point in a shape, this can be done from the child transform, but this method is a little tidier
///as it doesnt rely on checking a transform.tag each time. 
public class ShapePoints
{
    public Vector3 position;
    public bool invalid;
    public int mode; //this can be 1, 2 or 3, based on the "tag" of the object, shapePlace, shapeInvalid, shapeDontPlace
}

public class ShapeManager : MonoBehaviour
{
    public ShapeTemplate[] shapes;
    //completed checks can be removed once we have the execution order sorted and this script runs in the correct sequence.
    bool completedChecks = false;
    
    [Header("Draw Flags of obstacle positions")]
    public bool spawnFlags = false; 
    public GameObject obstacleFlag;
    
    //update this once execution order is complete, this is an awful method for executing scripts :D
    private void Update() { if (!completedChecks && GlobalStaticVariables.Instance.obstacleGenerationComplete) { CheckShapes(); } }


    void CheckShapes()
    {
        List<NodeObject> pathNodes = new List<NodeObject>();
        completedChecks = true;
        pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
        List<Vector3> placedPositions = new List<Vector3>();
        List<Vector3> nodePositions = new List<Vector3>();
               
        // assign the unwalkable pathNodes to nodePositions and the walkable pathNodes to pathPositions
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (!pathNodes[i].walkable)
            {
                nodePositions.Add(pathNodes[i].position);
                if (spawnFlags)
                {
                    Instantiate(obstacleFlag, pathNodes[i].position, Quaternion.identity);
                }
            }
        }

        foreach (ShapeTemplate currentShape in shapes) // iterate through all the shapes
        {
            if (currentShape.includeInBuild) //only build the shapes that are selected in the inspector
            {
            ShapePoints[] thisShape = new ShapePoints[currentShape.shape.transform.childCount]; //build and initialise an array to store the object points
            for(int x = 0; x < thisShape.Length; x++) { thisShape[x] = new ShapePoints(); }
            
             int checkedAllChildren = currentShape.shape.transform.childCount;  //keep track of how many children need to be checked in total, this avoids a false positive

                foreach (Vector3 nodePosition in nodePositions) 
                {
                    if (!placedPositions.Contains(nodePosition))                //dont check positions that already have a valid object placed on them.
                    {
                        int currentChildChecks = 0;                             //reset the checking variables
                        int count = 0;                                        
                        currentShape.shape.transform.position = nodePosition;   //move the check shape to the next nodePosition



                        ///build the template at the current checking position, assign 'modes' to each unit of the template to determine where objects can be placed
                        int j = 0;                                              //this little bit sucks, but it avoids checking obstacle tags later
                        foreach (Transform pos in currentShape.shape.transform)
                        {
                            thisShape[j].position = pos.position;
                            switch(pos.tag)
                            {
                                case "shapeValid":                                    thisShape[j].mode = 1;                                    break;
                                case "shapeDontPlace":                                thisShape[j].mode = 2;                                    break;
                                case "shapeInvalid":                                  thisShape[j].mode = 3;                                    break;
                                default:                                              thisShape[j].mode = 3;                                    break;
                                //note on default: this should never be called, and obstacle generation would have bigger problems, but in case a tag is missing, then make the item unplaceable.
                            } 
                            j++;
                        }                                                       //end of sucking
                        ///building template finished
                        


                        //check all children in the current shape, if they are of the invalid type (the object is not in a placeable position) then move on to the next shape
                        for (int i = 0; i < checkedAllChildren; i++)
                        {
                          
                            //if the shape is an invalid shape, and the position of it is also a position within the nodePositions generated by obstacle manager then break
                            if (thisShape[i].mode == 3)
                            {
                                if (nodePositions.Contains((thisShape[i].position))) { break; }
                            }
                            //if the shape is a valid position, the node is a valid placement position and there hasnt already been an object placed in its spot, then increment the counter
                            else if (thisShape[i].mode == 1 && nodePositions.Contains(thisShape[i].position) && !placedPositions.Contains(thisShape[i].position))
                            {
                                count++;
                            }
                            //if the shape is a valid position but an obstacle is not to be placed and there hasnt already been an object placed in its spot, then increment the counter
                            //note, this hasnt been included in the previous if statement because it would have been just as long as 2 separate ones, the OR conditions requires that the 
                            //full list of variables be written out for each 'mode'
                            else if(thisShape[i].mode == 2 && nodePositions.Contains(thisShape[i].position) && !placedPositions.Contains(thisShape[i].position))
                            {
                                count++;
                            }
                            

                            //keep track of how many children have been checked, this is important because you can have a false postive if only the valid children are checked and not the invalid too.
                            currentChildChecks++;
                        }

                        //ok, so have we had enough positive hits(count) and have we checked all of the children, including the invalid ones? if so, make the object. 
                        if (count == currentShape.unitCount && currentChildChecks == checkedAllChildren)
                        {
                            Instantiate(currentShape.model, currentShape.shape.transform.position, currentShape.model.transform.rotation);
                            //add each child shape to the placedPositions list to avoid anything being placed on top of them
                            foreach (ShapePoints shape in thisShape)
                            {
                                if (shape.mode == 1) { placedPositions.Add(shape.position); }
                            }
                        }
                    }
                }
            }
        }
    }
}
