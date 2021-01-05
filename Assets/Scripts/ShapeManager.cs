
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
    public ShapeTemplate[] shapes;
    //completed checks can be removed once we have the execution order sorted and this script runs in the correct sequence.
    bool completedChecks = false;

    [Header("Draw Flags of obstacle positions")]
    public bool spawnFlags = false;
    public GameObject obstacleFlag;
    int rotation = 90;
    //update this once execution order is complete, this is an awful method for executing scripts :D
    //  private void Update() { if (!completedChecks && GlobalStaticVariables.Instance.obstacleGenerationComplete) { CheckShapes(); } }



    public void CheckShapes()
    {
        List<NodeObject> pathNodes = new List<NodeObject>();
        completedChecks = true;
        pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
        List<Vector3Int> placedPositions = new List<Vector3Int>();
        List<Vector3Int> nodePositions = new List<Vector3Int>();
        List<ShapePoints> shapePoints = new List<ShapePoints>();
        List<Vector3Int> walkNodes = new List<Vector3Int>();

        // assign the unwalkable pathNodes to nodePositions and the walkable pathNodes to pathPositions
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (!pathNodes[i].walkable)
            {
                nodePositions.Add(Vector3Int.RoundToInt(pathNodes[i].position));
                if (spawnFlags)
                {
                    Instantiate(obstacleFlag, pathNodes[i].position, Quaternion.identity);
                }
            }
            if(pathNodes[i].walkable)
            {
                walkNodes.Add(Vector3Int.RoundToInt(pathNodes[i].position));
            }
        }
        Debug.Log("WALKNODES : " + walkNodes.Count);
        Array.Sort(shapes, delegate (ShapeTemplate x, ShapeTemplate y) { return x.unitCount.CompareTo(y.unitCount); });
        Array.Reverse(shapes);

        foreach (ShapeTemplate currentShape in shapes)
        {
            if (currentShape.includeInBuild)
            {
                GameObject tempRule = Instantiate(currentShape.rule);
                foreach (Vector3Int nodePosition in nodePositions)
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
                                    case "shapeValid": tempShape.mode = 1; break;
                                    case "shapeDontPlace": tempShape.mode = 2; break;
                                    case "shapeInvalid": tempShape.mode = 3; break;
                                    case "shapeNoPath": tempShape.mode = 4; break;
                                    default: tempShape.mode = 3; break;
                                }
                                shapePoints.Add(tempShape);
                            }

                            int count = 0;
                            foreach (ShapePoints checkPosition in shapePoints)
                            {
                                if(checkPosition.mode == 4 && walkNodes.Contains(checkPosition.position))
                                {
                                    count = -200; //silly value, probably a better way to ensure failure
                                    break;
                                }   
                                if (checkPosition.mode == 3 && nodePositions.Contains(checkPosition.position))
                                {
                                    count = -200; //silly value, probably a better way to ensure failure
                                    break;
                                }
                                if (checkPosition.mode == 1 || checkPosition.mode == 2)
                                {
                                    if (nodePositions.Contains(checkPosition.position) && !placedPositions.Contains(checkPosition.position))
                                    count++;
                                }
                                
                            }

                            if (count == currentShape.unitCount) //have we got enough positive hits to build this object? if so, make the model and add the points to the placedPositions list so they are unavailable
                            {
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
    }
/*using System.Collections;
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
    public ShapeTemplate[] shapes;
    //completed checks can be removed once we have the execution order sorted and this script runs in the correct sequence.
    bool completedChecks = false;
    
    [Header("Draw Flags of obstacle positions")]
    public bool spawnFlags = false; 
    public GameObject obstacleFlag;
    int rotation = 90;
    //update this once execution order is complete, this is an awful method for executing scripts :D
  //  private void Update() { if (!completedChecks && GlobalStaticVariables.Instance.obstacleGenerationComplete) { CheckShapes(); } }



   public void CheckShapes()
    {
        List<NodeObject> pathNodes = new List<NodeObject>();
        completedChecks = true;
        pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
        List<Vector3Int> placedPositions = new List<Vector3Int>();
        List<Vector3Int> nodePositions = new List<Vector3Int>();
        List<ShapePoints> shapePoints = new List<ShapePoints>();

       
        // assign the unwalkable pathNodes to nodePositions and the walkable pathNodes to pathPositions
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (!pathNodes[i].walkable)
            {
                nodePositions.Add(Vector3Int.RoundToInt(pathNodes[i].position));
                if (spawnFlags)
                {
                    Instantiate(obstacleFlag, pathNodes[i].position, Quaternion.identity);
                }
            }
        }
        Array.Sort(shapes, delegate (ShapeTemplate x, ShapeTemplate y) { return x.unitCount.CompareTo(y.unitCount); });
        Array.Reverse(shapes);
        
        foreach (ShapeTemplate currentShape in shapes)
        {
            if (currentShape.includeInBuild)
            {
                GameObject tempRule = Instantiate(currentShape.rule);
                foreach (Vector3Int nodePosition in nodePositions)
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
                                    case "shapeValid": tempShape.mode = 1; break;
                                    case "shapeDontPlace": tempShape.mode = 2; break;
                                    case "shapeInvalid": tempShape.mode = 3; break;
                                    default: tempShape.mode = 3; break;
                                }
                                shapePoints.Add(tempShape);
                            }

                            int count = 0;
                            foreach (ShapePoints checkPosition in shapePoints)
                            {
                                
                                if (checkPosition.mode == 3 && nodePositions.Contains(checkPosition.position))
                                {
                                    count = -200; //silly value, probably a better way to ensure failure
                                    i = rotation + 1;
                                    break;
                                }
                                if (checkPosition.mode == 1 || checkPosition.mode == 2)
                                {
                                    if (nodePositions.Contains(checkPosition.position) && !placedPositions.Contains(checkPosition.position))
                                        count++;
                                }
                            }

                            if (count == currentShape.unitCount) //have we got enough positive hits to build this object? if so, make the model and add the points to the placedPositions list so they are unavailable
                            {
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
}*/


/*// WORKING 29/12/2020
 * 
 * 
 * 
 *    void CheckShapes()
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
        Array.Sort(shapes, delegate (ShapeTemplate x, ShapeTemplate y) { return x.unitCount.CompareTo(y.unitCount); });
        Array.Reverse(shapes);

        foreach (ShapeTemplate currentShape in shapes) // iterate through all the shapes
        {
            if (currentShape.includeInBuild) //only build the shapes that are selected in the inspector
            {
            ShapePoints[] thisShape = new ShapePoints[currentShape.rule.transform.childCount]; //build and initialise an array to store the object points
            for(int x = 0; x < thisShape.Length; x++) { thisShape[x] = new ShapePoints(); }
            
             int checkedAllChildren = currentShape.rule.transform.childCount;  //keep track of how many children need to be checked in total, this avoids a false positive
                //currentShape.rule.transform.Rotate(currentShape.rule.transform.rotation * new Vector3(0, 90, 0));
                foreach (Vector3 nodePosition in nodePositions) 
                {
                    if (!placedPositions.Contains(nodePosition))                //dont check positions that already have a valid object placed on them.
                    {
                        int currentChildChecks = 0;                             //reset the checking variables
                        int count = 0;                                        
                        currentShape.rule.transform.position = nodePosition;   //move the check shape to the next nodePosition
                        


                        ///build the template at the current checking position, assign 'modes' to each unit of the template to determine where objects can be placed
                        int j = 0;                                              //this little bit sucks, but it avoids checking obstacle tags later
                        foreach (Transform pos in currentShape.rule.transform)
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
                            Instantiate(currentShape.model, currentShape.rule.transform.position, currentShape.rule.transform.rotation);
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
 * 
 * 
 * 
 * 
 * 
 * 
 * 
void CheckShapes()
{
    List<NodeObject> pathNodes = new List<NodeObject>();
    completedChecks = true;
    pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
    List<Vector3> placedPositions = new List<Vector3>();
    List<Vector3> nodePositions = new List<Vector3>();
    int rotations;               
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
    Array.Sort(shapes, delegate (ShapeTemplate x, ShapeTemplate y) { return x.unitCount.CompareTo(y.unitCount); });
    Array.Reverse(shapes);


        foreach (ShapeTemplate currentShape in shapes) // iterate through all the shapes
        {
        //do we rotate or not? if we dont, just run the for loop once
        rotations = 1;
        if(currentShape.rotate)            {                rotations = 4;            }


            if (currentShape.includeInBuild) //only build the shapes that are selected in the inspector
            {
            for (int rotLoop = 0; rotLoop < rotations; rotLoop++) //Loop 4 times to rotate the rule
            {
                GameObject tempRule = Instantiate(currentShape.rule);//, Vector3.zero, currentShape.rule.transform.rotation);
                ShapePoints[] thisShape = new ShapePoints[currentShape.rule.transform.childCount]; //build and initialise an array to store the object points
                for (int x = 0; x < thisShape.Length; x++) { thisShape[x] = new ShapePoints(); }

                int checkedAllChildren = currentShape.rule.transform.childCount;  //keep track of how many children need to be checked in total, this avoids a false positive


                foreach (Vector3 nodePosition in nodePositions)
                {
                    if (!placedPositions.Contains(nodePosition))                //dont check positions that already have a valid object placed on them.
                    {

                        int currentChildChecks = 0;                             //reset the checking variables
                        int count = 0;
                        int failedCount = 0;
                        //            currentShape.rule.transform.position = nodePosition;   //move the check shape to the next nodePosition



                        tempRule.transform.position = nodePosition;
                        tempRule.transform.rotation = Quaternion.identity;
                        tempRule.transform.localRotation = Quaternion.Euler(0, rotLoop * 90, 0);//Rotate(0.0f,(float)(rotLoop*rotation),0,Space.Self);
                        Debug.Log("Rotation : " + (rotLoop * 90) + " Transform rotated to :" + tempRule.transform.rotation.ToString());

                        ///build the template at the current checking position, assign 'modes' to each unit of the template to determine where objects can be placed
                        int j = 0;                                              //this little bit sucks, but it avoids checking obstacle tags later
                        foreach (Transform pos in tempRule.transform)
                        {
                            thisShape[j].position = pos.position;
                            switch (pos.tag)
                            {
                                case "shapeValid": thisShape[j].mode = 1; break;
                                case "shapeDontPlace": thisShape[j].mode = 2; break;
                                case "shapeInvalid": thisShape[j].mode = 3; break;
                                default: thisShape[j].mode = 3; break;
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
                                failedCount++;
                            }
                            //if the shape is a valid position, the node is a valid placement position and there hasnt already been an object placed in its spot, then increment the counter
                            else if (thisShape[i].mode == 1 && nodePositions.Contains(thisShape[i].position) && !placedPositions.Contains(thisShape[i].position))
                            {
                                count++;
                            }
                            //if the shape is a valid position but an obstacle is not to be placed and there hasnt already been an object placed in its spot, then increment the counter
                            //note, this hasnt been included in the previous if statement because it would have been just as long as 2 separate ones, the OR conditions requires that the 
                            //full list of variables be written out for each 'mode'
                            else if (thisShape[i].mode == 2 && nodePositions.Contains(thisShape[i].position) && !placedPositions.Contains(thisShape[i].position))
                            {
                                count++;
                            }


                            //keep track of how many children have been checked, this is important because you can have a false postive if only the valid children are checked and not the invalid too.
                            currentChildChecks++;
                        }
                      //  Debug.Log("Count : " + count);
                        //ok, so have we had enough positive hits(count) and have we checked all of the children, including the invalid ones? if so, make the object. 
                        if (count == currentShape.unitCount && currentChildChecks == checkedAllChildren && failedCount == 0)
                        {
                            Instantiate(currentShape.model, tempRule.transform.position, tempRule.transform.rotation);


                            //add each child shape to the placedPositions list to avoid anything being placed on top of them
                            foreach (ShapePoints shape in thisShape)
                            {
                                if (shape.mode == 1) { placedPositions.Add(shape.position); }
                            }

                        }
                     //   Debug.Log("Rotation : " + rotLoop + " Position of Unit 0 : " + thisShape[0].position.ToString() + " Position of Unit 1 : " + thisShape[1].position.ToString());


                    }


                }

                if (!currentShape.rotate)
                {

                    break;

                } //no need to go through the other 3 angles

            }

        }

    }  
    }
}

    */