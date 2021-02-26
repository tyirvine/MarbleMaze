using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class MarbleBehaviour : MonoBehaviour
{

    // Audio
    [Header("Audio")]
    AudioSource audioSource;
    public float audioTriggerSpeed;
    public AudioSource spikeDeath;
    public AudioClip impact;
    public AudioClip levelFinish;
    public AudioClip deathSound;

    // Settings
    [Header("Settings")]
    [Range(0.5f, 1.5f)] public float scale = 1.25f;
    [Range(0.1f, 50f)] public float jumpPower = 14.4f;
    [Range(0.01f, 1f)] public float jumpCooldown = 0.1f;
    [Range(0.1f, 2f)] public float physicsResetTime = 1f;
    [Range(1f, 8f)] public float yAxisSpeedReduction = 4f;

    // State Objects
    float currentTime;
    bool isGrounded;
    [HideInInspector] public bool shieldPickup = false;

    // Public References
    [Header("References")]
    public ParticleSystem particle;

    // Private References
    float marbleRadius;
    Rigidbody myRigidbody;
    MeshRenderer marbleRenderer;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public StatsManager statsManager;
    Dictionary<Transform, LandmineHazard> objects = new Dictionary<Transform, LandmineHazard>();
    List<LandmineHazard> activeHazards = new List<LandmineHazard>();
    LandmineHazard[] landmineHazards;
    public float hazardActiveRadius = 10f;

    // Grab references
    private void Awake()
    {
        // Find references
        colliders = gameObject.GetComponents<SphereCollider>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        statsManager = GameObject.FindObjectOfType<StatsManager>();
        audioSource = GetComponent<AudioSource>();
        marbleRadius = GetComponent<SphereCollider>().radius;
        myRigidbody = GetComponent<Rigidbody>();
        marbleRenderer = GetComponent<MeshRenderer>();

        // Set scale
        gameObject.transform.localScale = gameObject.transform.localScale * scale;
        currentTime = Time.time + jumpCooldown;
    }

    /// <summary>This can be used whenever the marble explodes. Control how long it takes the explosion to happen with delay.</summary>
    public void DeathSequenceExplode(float delay = 0f)
    {
        // Remove player's life
        statsManager.RemoveLife();

        Invoke("DeathSequenceEffects", delay);
    }

    void DeathSequenceEffects()
    {
        // Death effects
        spikeDeath.Play();
        particle.Play();
        marbleRenderer.enabled = false;

        // Freeze rigidbody
        myRigidbody.constraints = RigidbodyConstraints.FreezeAll;

    }

    // TODO: Consider deprecating
    /// <summary>An alternate to DeathSequenceExplode where the marble doesn't explode.</summary>
    public void DeathSequence() => PlayAudio(deathSound);

    /// <summary>Triggered when the player respawns.</summary>
    public void RespawnSequence()
    {
        marbleRenderer.enabled = true;
        myRigidbody.constraints = RigidbodyConstraints.None;
    }

    // What does this play? 
    // the audio clip selected, its just a wrapper
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
            Invoke("ResetRigidBody", physicsResetTime);
        }
    }

    public void ResetRigidBody()
    {
        myRigidbody.velocity = new Vector3(0f, myRigidbody.velocity.y / yAxisSpeedReduction, 0f);

        // This will drop the marble perfectly, but it's almost too perfect
        // myRigidbody.angularVelocity = Vector3.zero;
    }

    // Checks if the player is hitting a wall and checks for the collision force
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        if (collisionForce.magnitude > audioTriggerSpeed && collision.gameObject.CompareTag("wallTile"))
        {
            PlayAudio(impact);
        }
    }

    // Causes the player to jump. Assigned for the input manager package
    public void Jump()
    {
        // Check to make sure the marble is grounded first
        // @bubzy-coding The reason I changed it back to a sphere cast is because the OnCollisionEnter sometimes doesn't
        // correctly identify ground which leaves the player stuck and unable to jump. - @tyirvine
        RaycastHit hit;
        Vector3 rayDirection = (transform.position - new Vector3(0f, 1f, 0f)).normalized;
        if (Physics.SphereCast(transform.position, marbleRadius, rayDirection, out hit, 1f))
            isGrounded = true;

        // Then add force
        if (isGrounded && Time.time > currentTime)
        {
            isGrounded = false;
            currentTime = Time.time + jumpCooldown;
            myRigidbody.AddForceAtPosition(new Vector3(0f, 1f, 0f) * jumpPower, transform.position, ForceMode.Impulse);
        }
    }

}
