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

    // State objects
    bool isGrounded = false;

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

    /// <summary>This can be used whenever the marble explodes.</summary>
    public void DeathSequence()
    {
        GetComponent<MeshRenderer>().enabled = false;
        PlayAudio(deathSound);
    }

    // What does this play?
    public void PlayAudio(AudioClip _clip) => audioSource.PlayOneShot(_clip);

    // Checks for pickups and level finish
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

    // Checks if the player is hitting a wall and checks for the collision force. As well it checks if the marble is grounded
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        if (collisionForce.magnitude > audioTriggerSpeed && collision.gameObject.CompareTag("wallTile"))
        {
            PlayAudio(impact);
        }

        isGrounded = true;
    }

    // Checks if the marble is not grounded.
    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }

    // Causes the player to jump. Assigned for the input manager package
    void OnJump()
    {
        // Check to make sure the marble is grounded first
        if (isGrounded)
        {
            // Then add force
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForceAtPosition(new Vector3(0f, 1f, 0f) * jumpPower, transform.position, ForceMode.Impulse);
        }
    }
}
