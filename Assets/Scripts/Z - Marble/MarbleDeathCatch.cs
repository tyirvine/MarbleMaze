using System.ComponentModel;
using UnityEngine;

public class MarbleDeathCatch : MonoBehaviour
{
    // When the player enters the catch it removes a life
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatsManager statsManager = FindObjectOfType<StatsManager>();
            statsManager.RemoveLife();
        }
    }
}
