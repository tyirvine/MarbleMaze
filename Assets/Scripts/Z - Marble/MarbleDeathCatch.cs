using UnityEngine;

public class MarbleDeathCatch : MonoBehaviour
{
    // References
    MarbleBehaviour marble;
    PlayerInput input;

    // Grab references
    private void Awake()
    {
        marble = FindObjectOfType<MarbleBehaviour>();
        input = FindObjectOfType<PlayerInput>();
    }

    // When the player enters the catch it removes a life
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            input.PauseControls(true);
            marble.DeathSequenceExplode(0.35f);
        }
    }
}
