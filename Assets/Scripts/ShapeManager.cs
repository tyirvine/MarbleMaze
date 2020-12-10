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
    public int numberOfChecks;
    public Vector3Int currentPosition;
    public bool rotate;


    public ShapeTemplate()
    {
        currentPosition = new Vector3Int(0, 0, 0);
    }
}


public class ShapeManager : MonoBehaviour
{

    public ShapeTemplate[] shapes;

    
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (ShapeTemplate shapeTemplate in shapes)
        {
            Instantiate(shapeTemplate.shape.gameObject, shapeTemplate.currentPosition, shapeTemplate.shape.transform.localRotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
