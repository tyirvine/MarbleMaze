using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    // References 
    [HideInInspector] public MarbleBehaviour marble;

    // Grab stats manager
    private void Start()
    {
        marble = FindObjectOfType<MarbleBehaviour>();
    }

    // Detect for ball collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            marble.DeathSequenceExplode();
        }
    }
}
