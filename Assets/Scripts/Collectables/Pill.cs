using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill : MonoBehaviour
{
    public float rotSpeed;
    public Pickup pickup = new Pickup();
    // Start is called before the first frame update
    void Start()
    {
        pickup.pickupValue = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.parent)
        {
            GameObject parentMe = GameObject.FindGameObjectWithTag("boardObjects");
            transform.parent = parentMe.transform;
        }

        Vector3 rotation = (Vector3.up * rotSpeed * Time.deltaTime);
        transform.Rotate(rotation,Space.World);
    }

    
}
