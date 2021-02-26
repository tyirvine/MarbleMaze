
using UnityEngine;

public class Key : MonoBehaviour
{
    Gate gate;

    // Finds reference to the gate
    private void Start()
    {
        if (gate == null)
        {
            gate = FindObjectOfType<Gate>();
        }
    }

    // Destroys the gate
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gate.GotKey();
            Destroy(gameObject);
        }
    }
}
