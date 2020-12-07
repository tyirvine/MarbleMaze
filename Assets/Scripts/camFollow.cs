using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camFollow : MonoBehaviour
{
     public Transform target;
     public Vector3 offset; 

    public float smoothSpeed = 0.125f;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate ()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
     //   Vector3 desiredPosition = target.position + offset; 
      //  Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
       // transform.position = smoothPosition;

        transform.LookAt(target);
    }
}
