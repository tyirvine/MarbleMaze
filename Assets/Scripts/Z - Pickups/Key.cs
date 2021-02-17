using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    Gate gate;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }
    private void Update()
    {
        if(gate==null)
        {
            gate = FindObjectOfType<Gate>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gate.GotKey();
            Destroy(gameObject);
        }
    }
}
