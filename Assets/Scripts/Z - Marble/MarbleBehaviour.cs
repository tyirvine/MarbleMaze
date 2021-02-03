using UnityEngine;

public class MarbleBehaviour : MonoBehaviour
{
    [HideInInspector] public int score = 0;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public GameManager gameManager;
    [Range(0.5f, 1.5f)] public float scale = 1.25f;
    public float audioTriggerSpeed;
    
    AudioSource audioSource;
    public AudioClip impact;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Find references
        colliders = gameObject.GetComponents<SphereCollider>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        // Set scale
        gameObject.transform.localScale = gameObject.transform.localScale * scale;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > audioTriggerSpeed)
            {
                audioSource.clip = impact;
                audioSource.PlayOneShot(impact);
            }

    }
}
