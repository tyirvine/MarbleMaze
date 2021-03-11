using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    public Collider finishCollider;
    public AudioSource audioLevelFinish;

    GameManager gameManager;
    MarbleBehaviour marble;

    // State objects
    public bool disableCollider = false;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        marble = FindObjectOfType<MarbleBehaviour>();
    }

    public void ResetMarbleRigidBody() => marble.ResetRigidBody();

    // Detect marble
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && disableCollider == false)
        {
            disableCollider = true;
            // Make next level
            // audioLevelFinish.Play();
            gameManager.CallForNewBoard();
            // Invoke("ResetMarbleRigidBody", marble.physicsResetTime);

            // Disable collider
            // finishCollider.enabled = false;
        }
    }

}
