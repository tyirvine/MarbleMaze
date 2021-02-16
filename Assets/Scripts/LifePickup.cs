using System.Globalization;
using UnityEngine;

public class LifePickup : MonoBehaviour
{
    // References
    StatsManager stats;
    public ParticleSystem particle;
    public MeshRenderer meshRenderer;
    public Collider collider;
    [Range(0f, 10f)] public float time;

    // Audio Settings 
    [Header("Audio")]
    public AudioSource pickupSound;

    // Grab stats object
    private void Start()
    {
        stats = FindObjectOfType<StatsManager>();
    }

    // Add life when the player touches self (Add Life, Turn off Mesh, Play Audio, Run Particle State, Destroy)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stats.AddLife();
            pickupSound.Play();
            marbleState();
            particleState();
            Destroy(gameObject, time);
        }
    }

    private void particleState()
    {
        particle.Play();
        particle.loop = false;
    }

    private void marbleState()
    {
        meshRenderer.enabled = false;
        collider.enabled = false;
    }
}
