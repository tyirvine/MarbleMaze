using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballTest : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rigidbody;
    AudioSource audio;
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        audio = gameObject.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rigidbody.velocity.magnitude >0)
        {
            Debug.Log("Moving");
            if (!audio.isPlaying)
            {
                audio.Play();
                
            }
            audio.volume = rigidbody.velocity.magnitude/20f;
        }
        else
        {
            audio.Pause();
        }
    }
}
