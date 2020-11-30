using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
      public Transform destination;
    public float distance = 1f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, destination.position) <= distance)
        {
            Debug.Log("HERE");
        }
    }
}
