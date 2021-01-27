using UnityEngine;

public class MarbleBehaviour : MonoBehaviour
{
    [HideInInspector] public int score = 0;
    [HideInInspector] public Collider[] colliders;
    public GameManager gameManager;

    private void Start()
    {
        colliders = gameObject.GetComponents<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("pickup"))
        {
            score += other.GetComponent<Pill>().pickup.pickupValue;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("LevelFinish"))
        {
            gameManager.CallForNewBoard();
        }
    }
}
