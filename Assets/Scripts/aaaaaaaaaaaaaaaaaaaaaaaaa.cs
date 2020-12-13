using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaaaaaaaaaaaaaaaaaaaaaaaa : MonoBehaviour
{
    public GameObject cubeMove;
    // Start is called before the first frame update
    GameObject obj;
    Rigidbody rigidbozdy;
    void Start()
    {
        obj = Instantiate(cubeMove, new Vector3(0, 0, 0), Quaternion.identity);
        
        rigidbozdy = obj.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            
            rigidbozdy.position += new Vector3(0, 1, 0);
            
        }
    }
}
