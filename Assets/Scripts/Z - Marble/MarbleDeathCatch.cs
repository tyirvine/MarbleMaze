using UnityEngine;

public class MarbleDeathCatch : MonoBehaviour
{
    // References
    MarbleBehaviour marble;

    // Grab references
    private void Awake()
    {
        marble = FindObjectOfType<MarbleBehaviour>();
    }

    // When the player enters the catch it removes a life
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            marble.DeathSequenceExplode(0.35f);
        }
    }
}
