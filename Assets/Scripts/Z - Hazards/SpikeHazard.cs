using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    // References 
    public StatsManager statsManager;
    [HideInInspector] public MarbleBehaviour marble;

    // Grab stats manager
    private void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
        marble = FindObjectOfType<MarbleBehaviour>();
    }

    // Detect for ball collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            marble.DeathSequence();
            statsManager.RemoveLife();
        }
    }
}
