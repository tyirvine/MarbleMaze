using UnityEngine;
using System.Collections.Generic;
public class MarbleBehaviour : MonoBehaviour
{
    [HideInInspector] public int score = 0;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public GameManager gameManager;
    [Range(0.5f, 1.5f)] public float scale = 1.25f;
    public float audioTriggerSpeed;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip impact;
    public AudioClip levelFinish;
    public AudioClip deathSound;

    public int fallDistance = 30;   //how far from the board does the marble fall before triggering a death event
    bool fellToMyDeath = false;     //stops the falling check, it was previously causing a lot of level skipping

    public List<GameObject> oldTiles = new List<GameObject>();
    
    public int tilesBeforeDeletion = 5; //how many tiles we touch before deleting them

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
        if(collision.gameObject.CompareTag("bomb"))
        {
            collision.gameObject.SendMessage("Explode");
        }

        if(collision.gameObject.CompareTag("spike"))
        {
            PlayAudio(deathSound);    
            Debug.Log("this is where we lose a life");
        }

        if(collision.gameObject.tag == "floorTile")
        {
            oldTiles.Add(collision.gameObject);
        }
    }

    public void PlayAudio(AudioClip _clip)
    {
        audioSource.PlayOneShot(_clip);
    }


    public void ResetState()
    {
        fellToMyDeath = false;
        oldTiles.Clear();
    }




    public void Update()
    {
        //has the player fallen off the board?
        //run this check in update as its not physics heavy
        if (gameManager.boardPosition != null)
        {
            if (transform.position.y < gameManager.boardPosition.position.y - fallDistance && !fellToMyDeath)
            {
                Debug.Log("die");
                fellToMyDeath = true;
                gameManager.RemoveLife();
                gameManager.CallForNewBoard();
            }
        }
        if(oldTiles.Count >= tilesBeforeDeletion && oldTiles.Count >0 && oldTiles[0]!=null)
        {
            oldTiles[0].GetComponent<Rigidbody>().isKinematic = false;
            oldTiles[0].GetComponent<Rigidbody>().useGravity = true;
            Destroy(oldTiles[0].gameObject, 2);
            oldTiles.RemoveAt(0);            
        }
    }

}
