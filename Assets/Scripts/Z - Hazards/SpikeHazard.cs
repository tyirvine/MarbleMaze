using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    // References 
    public StatsManager statsManager;

    // Grab stats manager
    private void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
    }

    // Detect for ball collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<MeshRenderer>().enabled = false;
            statsManager.RemoveLife();
        }
    }
}
