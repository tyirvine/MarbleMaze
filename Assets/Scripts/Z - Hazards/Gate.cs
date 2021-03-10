using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public ParticleSystem particleDisappear;
    public ParticleSystem particleIdle;
    public Collider colliderGate;
    public Behaviour halo;
    MeshRenderer[] meshRenderers;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    void DelayDestroy()
    {
        particleIdle.Stop();
        colliderGate.enabled = false;
        halo.enabled = false;

        // Disable all mesh renderers
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = false;
        }
    }

    public void GotKey()
    {
        particleDisappear.Play();
        Invoke(nameof(DelayDestroy), 0.1f);
        Destroy(gameObject, 5f);
    }
}
