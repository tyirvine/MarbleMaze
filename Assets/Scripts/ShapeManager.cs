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
    public int numberOfChecks;
    public Vector3Int currentPosition;
    public int unitCount; //how many squares to make this object?
    public int allowedGaps = 0; //how many gaps are allowed? (THIS WILL AFFECT LEVEL TRAVERSAL AS IT DOESNT CARE ABOUT A PATH CURRENTLY!!!!!!!!)

    public ShapeTemplate()
    {
        currentPosition = new Vector3Int(0, 0, 0);
    }
}


public class ShapeManager : MonoBehaviour
{
    public ShapeTemplate[] shapes;
    bool completedChecks = false;

    List<Vector3> obstacles;
    List<Vector3> confirmedShapes = new List<Vector3>();
    List<Vector3> templateOrigin = new List<Vector3>(); // for the parent transform of each object (may be useful if we are instantiating models instead of units)

    
    private void Start()
    {
       
        
    }

    private void Update()
    {
        //run this after everything else
        if(!completedChecks && GlobalStaticVariables.Instance.obstacleGenerationComplete)
        {
            checkShapes();
            completedChecks = true;
        }
    }

    void checkShapes()
    {
        GameObject[] tempObstacles = GameObject.FindGameObjectsWithTag("pathObstacle");
        obstacles = new List<Vector3>();
        foreach (GameObject obstacle in tempObstacles)
        {
            obstacles.Add(obstacle.transform.position);
        }
        Debug.Log(obstacles.Count + " obstacles in list");

        //this function currently only checks obstacles, its possible we could add the path too in the preceding lines
        bool checkUnitAgainstList(Vector3 position)
        {
            if (obstacles.Contains(position))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        int count = 0;
        foreach(ShapeTemplate shape in shapes)
        {
            for (int i = -GlobalStaticVariables.Instance.gridXSizeHalfLength; i < GlobalStaticVariables.Instance.gridXSizeHalfLength; i++)
            {

                for (int j = -GlobalStaticVariables.Instance.gridZSizeHalfLength; j < GlobalStaticVariables.Instance.gridZSizeHalfLength; j++)
                {
                    //move the shape to the new position in the grid
                    shape.shape.transform.position = new Vector3(i, 0, j);
                    //check each child object (unit) in the shape
                  //  int checkCount = 0;
                    foreach (Transform child in shape.shape.transform)
                    {
                  //   checkCount++;
                  //   Debug.Log("Check Count : "+checkCount);
                    //does the list contain the position of the child? if it does mark it on another list as a constructable object
                        if (checkUnitAgainstList(child.position))
                        {
                            count++;
                            Debug.Log("count : " + count);
                            //have we got as many positive hits from the object list as the reference number of units in this shape
                            if(count == shape.unitCount-shape.allowedGaps)
                            {
                                //add all of the child positions to the list of confirmed shapes
                                //NOTE : this will have to be more detailed later, perhaps store only the parent position and instantiate a model of the shape in its place,
                                //or different lists for different shapes. anyway the position can be stored in multiple ways so we can work it out.
                                templateOrigin.Add(shape.shape.transform.position);
                                Instantiate(shape.model, shape.shape.transform.position, Quaternion.identity);
                                Debug.Log(shape.model.name);

                                foreach(Transform childPos in shape.shape.transform)
                                {
                                    confirmedShapes.Add(childPos.position);
                                    obstacles.Remove(childPos.position);
                                }
                              

                            }
                        }
                    }
                    count = 0;
                }
            }            
        }
        
        
        
    }
}




/*

public class ShapeManager : MonoBehaviour
{
    public ShapeTemplate[] shapes;

    private void Start()
    {
        Debug.Log("2");
        checkShapesAgainstObstacles();
    }

    public void checkShapesAgainstObstacles()
    {
        Debug.Log("3");
        foreach(ShapeTemplate shape in shapes)
        {
        //  Debug.Log("pre instantiate");
            GameObject temp = Instantiate(shape.shape, new Vector3Int(0, 10, 0), Quaternion.identity); //instantiate at a position that wont interfere with checks

            Rigidbody rigidbod = temp.GetComponent<Rigidbody>();
            Debug.Log("5");
            Debug.Log("Rigidbody Postion " + rigidbod.position.ToString());
            for(int i = -globalStaticVariables.Instance.gridXSizeHalfLength; i < globalStaticVariables.Instance.gridXSizeHalfLength; i++)
            {
                
               for(int j = -globalStaticVariables.Instance.gridZSizeHalfLength; j < globalStaticVariables.Instance.gridZSizeHalfLength; j++)
                {
                    //Debug.Log("CALLING MOVEMENT : i : " + i + " j : " + j );
                    temp.SetActive(true);
                    //temp.transform.position = new Vector3(i, 0, j);
                    rigidbod.position = new Vector3(i, 0, j);
                    temp.transform.name = "BIG DOG";
                    Debug.Log("Rigidbody Postion " + rigidbod.position.ToString());
                    temp.SetActive(false);
                    
                    //Debug.Log(temp.transform.position);
                    // shape.shape.SetActive(true);
                }
           }
            //Destroy(temp);
        }

    }
}
/*
    public ShapeTemplate[] shapes;

    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void runObstacleCheck()
    {

        Vector3Int startPosition = new Vector3Int(-globalStaticVariables.Instance.gridXSizeHalfLength, 0, -globalStaticVariables.Instance.gridZSizeHalfLength);
        foreach (ShapeTemplate shapeTemplate in shapes)
        {
            Vector3Int currentPosition = startPosition;
            /*  for (int i = -globalStaticVariables.Instance.gridXSizeHalfLength+1; i < globalStaticVariables.Instance.gridXSizeHalfLength; i += shapeTemplate.shape.GetComponent<shapeOverlap>().width)
              {
                  for (int j = -globalStaticVariables.Instance.gridZSizeHalfLength+1; j < globalStaticVariables.Instance.gridZSizeHalfLength; j += shapeTemplate.shape.GetComponent<shapeOverlap>().height)
                  {
                      GameObject tempShape = Instantiate(shapeTemplate.shape.gameObject, currentPosition, shapeTemplate.shape.transform.localRotation);
                      currentPosition = new Vector3Int(i, 0, j);
                  }
              }
            ///////
            for (int i = -globalStaticVariables.Instance.gridXSizeHalfLength - 1; i < globalStaticVariables.Instance.gridXSizeHalfLength; i++)
            {
                for (int j = -globalStaticVariables.Instance.gridZSizeHalfLength + 1; j < globalStaticVariables.Instance.gridZSizeHalfLength; j++)
                {
                    GameObject tempShape = Instantiate(shapeTemplate.shape.gameObject, currentPosition, shapeTemplate.shape.transform.localRotation);
                    currentPosition = new Vector3Int(i, 0, j);
                   // Destroy(tempShape);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
*/