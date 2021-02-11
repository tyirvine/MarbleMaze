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
    [Range(0.1f, 1f)] public float jumpCooldown = 0.1f;

    //Timer Stuff
    float currentTime;

    // References
    float marbleRadius;
    Rigidbody myRigidbody;

    // Grab references
    private void Awake()
    {
        // Find references
        colliders = gameObject.GetComponents<SphereCollider>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        marbleRadius = GetComponent<SphereCollider>().radius;
        myRigidbody = GetComponent<Rigidbody>();
        // Set scale
        gameObject.transform.localScale = gameObject.transform.localScale * scale;
        currentTime = Time.time + jumpCooldown;
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

    // Checks if the player is hitting a wall and checks for the collision force
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        if (collisionForce.magnitude > audioTriggerSpeed && collision.gameObject.CompareTag("wallTile"))
        {
            PlayAudio(impact);
        }
        if (collision.transform.CompareTag("floorTile") && !isGrounded)
        {
            isGrounded = true;
            Debug.Log("HitGround");
        }
    }

    // Causes the player to jump. Assigned for the input manager package
    public void Jump()
    {
        /*   // Check to make sure the marble is grounded first
           RaycastHit hit;
           Vector3 rayDirection = (transform.position - new Vector3(0f, 1f, 0f)).normalized;
           if (Physics.SphereCast(transform.position, marbleRadius, rayDirection, out hit, 1f))
           {
               // Then add force
               Rigidbody rigidbody = GetComponent<Rigidbody>();
               rigidbody.AddForceAtPosition(new Vector3(0f, 1f, 0f) * jumpPower, transform.position, ForceMode.Impulse);
           }
        */
        // Then add force
        if (isGrounded && Time.time > currentTime)
        {
            isGrounded = false;
            currentTime = Time.time + jumpCooldown;
            myRigidbody.AddForceAtPosition(new Vector3(0f, 1f, 0f) * jumpPower, transform.position, ForceMode.Impulse);
        }
    }


}
