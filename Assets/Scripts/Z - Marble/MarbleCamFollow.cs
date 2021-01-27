using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleCamFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public float smoothSpeed = 0.125f;
    private void Start()
    {


    }
    void FixedUpdate()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, 0, target.position.z) + offset;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothPosition;
        }
        //transform.LookAt(target);
    }
}
