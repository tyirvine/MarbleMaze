using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballTest : MonoBehaviour
{
    // Start is called before the first frame update
    
    
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Rigidbody>().velocity.magnitude >0)
        {
            Debug.Log("Moving");
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Play();
            }
            GetComponent<AudioSource>().volume = GetComponent<Rigidbody>().velocity.magnitude/20f;
        }
        else
        {
            GetComponent<AudioSource>().Pause();
        }
    }
}
