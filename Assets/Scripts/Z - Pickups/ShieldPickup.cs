using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    StatsManager stats;
    [Range(1, 10)] public float time;
    [HideInInspector] public MarbleBehaviour marble;

    public AudioSource pickupSound;
    public ParticleSystem particle;
    public SphereCollider sphereCollider;
    public MeshRenderer[] meshRenderers;

    public void Start()
    {
        marble = FindObjectOfType<MarbleBehaviour>();
        stats = FindObjectOfType<StatsManager>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            marble.shieldPickup = true;
            pickupSound.Play();
            foreach (MeshRenderer mesh in meshRenderers)
            {
                mesh.enabled = false;
            };
            sphereCollider.enabled = false;
            ParticleState();
            Invoke("TurnOffShield", time);
            Destroy(gameObject, time);
        }
    }

    private void TurnOffShield()
    {
        marble.shieldPickup = false;
    }

    private void ParticleState()
    {
        particle.Play();
        particle.loop = false;
    }

}
