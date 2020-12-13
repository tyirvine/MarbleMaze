using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeUnit : MonoBehaviour
{

    public Vector3 oldPosition;
    private void Start()
    {
        globalStaticVariables.Instance.debugLog.Add("Started shapeUnit.ccs      Time Executed : " + Time.deltaTime.ToString());
        oldPosition = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Triggered");
        if(other.gameObject.CompareTag("pathObstacle"))
        {
            Destroy(other.gameObject);
            Debug.Log("Destroyed");
            SendMessageUpwards("addValue", 1);
        }
    }
    private void Update()
    {
        Debug.Log("Shape Unit Position " + transform.position);
    }
}
/*
    // Start is called before the first frame update
    public int hit = 0;
    Collider objectHit;
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pathObstacle"))
        {
            hit = 1;
            SendMessageUpwards("addValue", 1);
            objectHit = other;
            Destroy(other.gameObject);
            
        }
        Debug.Log("My vlaue is : " + hit);
    }

    public void CompletedCheck()
    {
        if (objectHit != null)
        {
            Destroy(objectHit.gameObject);
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
*/