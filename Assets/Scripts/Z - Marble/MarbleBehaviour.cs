using System;
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
    public AudioClip deathSound;

    // Settings
    [Header("Settings")]
    [Range(0.5f, 1.5f)] public float scale = 1.25f;
    [Range(0.1f, 50f)] public float jumpPower = 14.4f;
    [Range(0.01f, 1f)] public float jumpCooldown = 0.1f;
    [Range(0.1f, 2f)] public float physicsResetTime = 1f;
    [Range(1f, 8f)] public float yAxisSpeedReduction = 4f;

    // Effects
    [Header("Effects")]
    public float invincibleTime = 10f;
    public float shieldTransitionDuration = 0.2f;

    // State Objects
    float currentTime;
    bool isGrounded;
    [HideInInspector] public bool shieldEnabled = false;

    // Public References
    [Header("References")]
    public ParticleSystem particle;
    public ParticleSystem particleShieldTransition;
    public Material materialShield;
    public Material materialBase;

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

    /* ----------------------------- Marble Effects ----------------------------- */

    /// <summary>Makes the marble invincible.</summary>
    public void MakeInvinicible()
    {
        // Cancel any disable invokes. This sort of resets the timer
        CancelInvoke(nameof(DisableInvincible));

        // Set shield to enabled
        shieldEnabled = true;

        // Change shader
        particleShieldTransition.Play();
        Invoke(nameof(ShowShieldMaterial), 0.2f);

        // Revert all after time
        Invoke(nameof(DisableInvincible), invincibleTime);
    }

    /// <summary>Used to delay the switch of mateirals.</summary>
    public void ShowShieldMaterial() => marbleRenderer.material = materialShield;
    public void HideShieldMaterial() => marbleRenderer.material = materialBase;

    /// <summary>Disables the invincibility.</summary>
    public void DisableInvincible()
    {
        shieldEnabled = false;
        particleShieldTransition.Play();
        Invoke(nameof(HideShieldMaterial), 0.2f);
    }

    /* ---------------------------- Marble Behaviours --------------------------- */

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
        // @tyirvine - I removed the timer and increased the nearness to the ground so it felt more playful rather than limiting
        RaycastHit hit;
        Vector3 rayDirection = new Vector3(0f, -1f, 0f);
        if (Physics.SphereCast(transform.position, marbleRadius * 1f, rayDirection, out hit, 0.5f))
        {
            isGrounded = false;
            currentTime = Time.time + jumpCooldown;
            myRigidbody.AddForceAtPosition(new Vector3(0f, 1f, 0f) * jumpPower, transform.position - new Vector3(0f, 1f, 0f), ForceMode.Impulse);
        }
    }

}
