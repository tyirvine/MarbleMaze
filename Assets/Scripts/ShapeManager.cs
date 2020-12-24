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

   
    void CheckShapes()
    {
        
        completedChecks = true;
        pathNodes.AddRange(gameObject.GetComponent<ObstacleManager>().tempNodes);
        List<Vector3> placedPositions = new List<Vector3>();
        List<Vector3> nodePositions = new List<Vector3>();
        //List<Vector3> pathPositions = new List<Vector3>();

        ///<summary>Check the input Vector 3 against 2 position lists to ensure theres no clash</summary>
        bool CheckForInvalidShape(Vector3 position) 
        {
            if (nodePositions.Contains(position)  || placedPositions.Contains(position))
            {
                
                return true;  //the position is in one of the lists, therefore it is not a valid placement position
            } 
            else            
            {
                return false;           
            }
        }

        // assign the unwalkable pathNodes to nodePositions and the walkable pathNodes to pathPositions
        for (int i = 0; i < pathNodes.Count; i++)
        {   
            if (!pathNodes[i].walkable)            
            {                
                nodePositions.Add(pathNodes[i].position);
                
            }
            else                                        
            {                
             //   pathPositions.Add(pathNodes[i].position);            
            }        
        }
        

        foreach(ShapeTemplate currentShape in shapes) // iterate through all the shapes
        {
            
            
            
            if (currentShape.includeInBuild) //only build the shapes that are selected in the inspector
            {
                int checkedAllChildren = currentShape.shape.transform.childCount; //keep track of how many children need to be checked in total, this avoids a false positive

                foreach (Vector3 nodePosition in nodePositions)
                {
                    int currentChildChecks = 0;
                    bool killChild = false;
                    int count = 0;
                    currentShape.shape.transform.position = nodePosition;
                    
                    if (!placedPositions.Contains(nodePosition)) //check that there isnt already an obstacle placed at this location
                    {
                        
             //           Debug.Log("Running : " + checkedAllChildren.ToString() + " checks on " + currentShape.shape.name);
                        foreach (Transform child in currentShape.shape.transform)
                        {
                            
                            if (child.CompareTag("shapeInvalid") && CheckForInvalidShape(child.position))
                            {
             //                   Debug.Log("Kill : Reached check #" + currentChildChecks + " on shape " + currentShape.shape.name);
                                killChild = true;
                                count = 0;
                                currentChildChecks = 0;
                                break;                                
                            }
                            if (nodePositions.Contains(child.position) && !placedPositions.Contains(child.position) && !child.CompareTag("shapeInvalid"))
                                {
                                    count++;
                                }
                                currentChildChecks++;
                            }
                        }
                        
                            if (count == currentShape.unitCount && currentChildChecks == checkedAllChildren && !killChild)
                            {
                                Instantiate(currentShape.model, currentShape.shape.transform.position + new Vector3(0, count, 0), currentShape.model.transform.rotation);
                                foreach (Transform child in currentShape.shape.transform)
                                {
                                    if (!child.CompareTag("shapeInvalid"))
                                        placedPositions.Add(child.position);
                                }

                            }
                    
                    }
                }
            }
        }
        

    }
