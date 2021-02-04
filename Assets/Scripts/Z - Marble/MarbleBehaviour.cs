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
    public AudioClip levelFinish;
    public Collision oldCollision;
    int layerMask = 1 << 9;
    private void Start()
    {
     
      //  layerMask = ~layerMask;
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
            audioSource.clip = levelFinish;
            audioSource.PlayOneShot(levelFinish);
            gameManager.CallForNewBoard();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {


        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        if (collisionForce.magnitude > audioTriggerSpeed && collision.gameObject.CompareTag("wallTile")) 
            {
                audioSource.clip = impact;
                audioSource.PlayOneShot(impact);
            
            
            }
        

    }
}
