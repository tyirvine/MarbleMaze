using UnityEngine;

public class LandmineGarbageCollector : MonoBehaviour
{
    // Just delete any old wall tiles
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            Destroy(other.gameObject, 5.0f);
    }
}
