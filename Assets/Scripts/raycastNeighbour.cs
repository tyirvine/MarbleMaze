using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycastNeighbour : MonoBehaviour
{
    public float rayRange = 2f;
    public GameObject tripleStraight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayRange, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * rayRange, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward) * rayRange, Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.right) * rayRange, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayRange))
        {                     
            Debug.Log(hit.transform.tag + "hit in the forwards direction");
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, rayRange))
        {
            Debug.Log(hit.transform.tag + "hit in the right direction");
            Debug.Log(Vector3.Distance(hit.transform.position, transform.position));
            //if(Vector3.Distance(hit.transform.position, transform.position)
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, rayRange))
        {
            Debug.Log(hit.transform.tag + "hit in the backwards direction");
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.right), out hit, rayRange))
        {
            Debug.Log(hit.transform.tag + "hit in the left direction");
        }
    }
}
