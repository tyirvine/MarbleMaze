
using UnityEngine;

public class Key : MonoBehaviour
{
    Gate gate;
    public ParticleSystem particlePickedup;
    public Behaviour halo;
    public Collider collider;
    public ParticleSystem particleIdle;

    // Finds reference to the gate
    private void Start()
    {
        if (gate == null)
        {
            gate = FindObjectOfType<Gate>();
        }
    }

    /// <summary>Delays the destory of the object.</summary>
    void DisableMesh()
    {
        halo.enabled = false;
        collider.enabled = false;
        particleIdle.Stop();
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Destroys the gate
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gate.GotKey();
            particlePickedup.Play();
            Invoke(nameof(DisableMesh), 0.1f);
            Destroy(gameObject, 10f);
        }
    }
}
