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

    // public void Update()
    // {
    //     // Has the player fallen off the board?
    //     // Run this check in update as its not physics heavy
    //     if (gameManager.boardPosition != null)
    //     {
    //         if (transform.position.y < (gameManager.boardPosition.position.y - fallDistanceToRemoveLife) && !gameManager.marbleIsFalling)
    //         {
    //         }
    //     }
    // }
}
