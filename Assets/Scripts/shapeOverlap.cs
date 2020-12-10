using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shapeOverlap : MonoBehaviour
{
    Transform[] shapeObjects;
    // Start is called before the first frame update
    void Start()
    {

        int childCount = transform.childCount;
        shapeObjects = new Transform[childCount];

        for (int i = 0; i < childCount;i++)
        {
            shapeObjects[i] = transform.GetChild(i);
                }
        checkShapeOverlap();
    }    

    void checkShapeOverlap()
    {

        int count = shapeObjects.Length;
        int currentCount = 0;
        foreach (Transform shapeObject in shapeObjects)
        {
            Collider[] colliders = Physics.OverlapBox(shapeObject.transform.position, shapeObject.transform.localScale / 8);
            foreach (Collider collider in colliders)
            {
                if (collider.transform.tag == "pathObstacle" && collider.enabled)
                {
                    shapeObject.gameObject.SetActive(false);
                    currentCount++;
                }
            else
                {
                    Debug.Log("No Colliders found at this location");
                }
                
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.A))
        {
            checkShapeOverlap();
        }
        if(Input.GetKeyUp(KeyCode.S))
        {
            foreach(Transform trans in shapeObjects)
            {
                trans.gameObject.SetActive(!trans.gameObject.activeSelf);
            }
        }
    }
}
