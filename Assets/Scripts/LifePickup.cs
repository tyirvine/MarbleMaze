
using UnityEngine;

public class LifePickup : MonoBehaviour
{
    // References
    StatsManager stats;

    // Grab stats object
    private void Start()
    {
        stats = FindObjectOfType<StatsManager>();
    }

    // Add life when the player touches self
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stats.AddLife();
            Destroy(gameObject);
        }
    }
}
