using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//make a list of obstaccle positions
//check the shapes in my shape against those positions
//same as collision detection but with numbers
public class shapeOverlap : MonoBehaviour
{
    Transform[] shapeObjects;

    //temporary object for debug. will be instantiated in place of the check flags
    //public GameObject tempCube;

    //[Tooltip("Set these for the shape manager to know how far to move the shape per check")]
   // public int width;       //the width in whole units of this object
   // public int height;      //the height in whole units of this object
   // public int value;       //the current value of the gameobject, this is determined by the number of positive hits from the child objects

    
    public int gapsAllowed = 0;     //experimental for making objects when the template doesnt quite fit

    bool didStartup = false;        //experimental if a function within this code is called from another script, start doesnt execute.

    Vector3 oldPos;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("4");
        oldPos = transform.position;
        if (!didStartup)
        {
            startupRoutine();
            didStartup = true;
        }
    }
    void startupRoutine()
    {
        
      //  Debug.Log("RUNNING OBSTACLE CHECK");
        globalStaticVariables.Instance.debugLog.Add("Started shapeOverlap.cs      Time Executed : " + Time.deltaTime.ToString());

        int childCount = transform.childCount;
        shapeObjects = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
         {
            shapeObjects[i] = transform.GetChild(i);
         }
    }

    

    private void Update()
    {
      //  Debug.Log(transform.position);
        if (transform.position != oldPos)
        {
            oldPos = transform.position;
            Debug.Log("MOVED L SHAPE");
            BroadcastMessage("moveUnit", transform.position);
        }
       
    }
}


    #region oldcode
/* public void addValue(int val)
 {
     if (!didStartup)
     {
         startupRoutine();
         didStartup = true;
     }
     value += val;
     if (value == shapeObjects.Length)
     {
         Debug.Log("Hit Value Cap at ");
         BroadcastMessage("CompletedCheck");
     }
     Debug.Log("Actual Value at end " + value);
     if(value > shapeObjects.Length)
     {
         transform.position += new Vector3(0, 1, 0);
     }
 }
*/

/*  void checkShapeOverlap()
  {

      int count = shapeObjects.Length;
      int currentCount = 0;
      List<Vector3Int> collectedFlags = new List<Vector3Int>();
      foreach (Transform shapeObject in shapeObjects)
      {
          Collider[] colliders = Physics.OverlapBox(shapeObject.transform.position, shapeObject.transform.localScale / 8);

          foreach (Collider collider in colliders)
          {
              if (collider.transform.CompareTag("pathObstacle") && collider.enabled)
              {

                  if(globalStaticVariables.Instance.debugMode)
                  {
                  }

                  collectedFlags.Add(new Vector3Int((int)collider.transform.position.x, (int)collider.transform.position.y,(int)collider.transform.position.z));
                  Destroy(collider.gameObject);
                  currentCount++;
                  Debug.Log("CurrentCount :  " + currentCount);
              }
          else
              {
                  Debug.Log("No Colliders found at this location");
              }


          }
      }
      if(currentCount == count)
      {

          foreach(Transform shapeObject in shapeObjects)
          {              
              Instantiate(tempCube,shapeObject.position,Quaternion.identity);
              Destroy(shapeObject.gameObject);                


          }
      }
      else
      {
          Debug.Log("not enough matches to create piece");
          foreach (Transform shapeObject in shapeObjects)
          {

              Destroy(shapeObject.gameObject);
          }


      }
      Destroy(gameObject);
  }

// Update is called once per frame
void Update()
{

}
}
*/
#endregion
