using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    [Range (1,10)]int time;
    [HideInInspector] public MarbleBehaviour marble;

    public void Start()
    {
        marble = FindObjectOfType<MarbleBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            marble.shieldPickup = true;
            Invoke("TurnOffShield", time);
        }
    }

    private void TurnOffShield()
    {
        marble.shieldPickup = false;
    }

}
