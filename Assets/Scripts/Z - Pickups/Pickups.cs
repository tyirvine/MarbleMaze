using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    public GameObject pickupEffect;
    public float multiplier = 1.4f;
    public float duration = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Pickup(other));
        }

    }

    IEnumerator Pickup(Collider player)
    {
        Instantiate(pickupEffect, transform.position, transform.rotation); // Create Effect


        // Different Pick ups start here
        player.transform.localScale *= multiplier; // Makes Player Size Change
        // player.GetComponent<MeshRenderer>().enabled = false; // Makes Player invisible
        Time.timeScale *= multiplier; // Makes Time Slow down 

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(duration);

        // Pick ups revert here
        player.transform.localScale /= multiplier; //Add Ability Here
        player.GetComponent<MeshRenderer>().enabled = true;
        Time.timeScale /= multiplier;

        Destroy(gameObject); // Destroy Game Object
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }
}
