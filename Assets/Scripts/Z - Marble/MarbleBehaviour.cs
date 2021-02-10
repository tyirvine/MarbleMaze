using UnityEngine;

public class MarbleBehaviour : MonoBehaviour
{
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public GameManager gameManager;

    [Header("Audio")]
    AudioSource audioSource;
    public float audioTriggerSpeed;
    public AudioClip impact;
    public AudioClip levelFinish;
    public AudioClip deathSound;

    [Header("Settings")]
    [Range(0.5f, 1.5f)] public float scale = 1.25f;
    [Range(0.1f, 50f)] public float jumpPower = 14.4f;

    // Grab references
    private void Awake()
    {
        // Find references
        colliders = gameObject.GetComponents<SphereCollider>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();

        // Set scale
        gameObject.transform.localScale = gameObject.transform.localScale * scale;
    }

    public void PlayAudio(AudioClip _clip)
    {
        audioSource.PlayOneShot(_clip);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("pickup"))
        {
            gameManager.statsManager.AddToScore(1);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("LevelFinish"))
        {
            PlayAudio(levelFinish);
            gameManager.CallForNewBoard();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        if (collisionForce.magnitude > audioTriggerSpeed && collision.gameObject.CompareTag("wallTile"))
        {
            PlayAudio(impact);
        }

        if (collision.gameObject.CompareTag("spike"))
        {
            PlayAudio(deathSound);
            Debug.Log("this is where we lose a life");
        }
    }

    // Causes the player to jump
    void OnJump()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForceAtPosition(new Vector3(0f, 1f, 0f) * jumpPower, transform.position, ForceMode.Impulse);
    }
}
