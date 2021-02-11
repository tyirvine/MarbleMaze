using UnityEngine;

public class MarbleDeathCatch : MonoBehaviour
{
    // References
    StatsManager statsManager;
    MarbleBehaviour marble;

    // Grab references
    private void Awake()
    {
        statsManager = FindObjectOfType<StatsManager>();
        marble = FindObjectOfType<MarbleBehaviour>();
    }

    // When the player enters the catch it removes a life
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            statsManager.RemoveLife();
            marble.DeathSequence();
        }
    }
}
