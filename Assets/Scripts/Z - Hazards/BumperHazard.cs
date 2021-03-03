
using UnityEngine;

public class BumperHazard : MonoBehaviour
{
    // References
    public float bumperForce = 5f;
    public LayerMask layerToInteract;
    public Animator animator;
    [HideInInspector] public Collider[] sphereColliders;

    [Header("Audio")]
    public AudioSource bumperSound;
    // Get colliders
    private void Awake()
    {
        sphereColliders = GetComponents<SphereCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((layerToInteract.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sphereColliders[0] == collision.contacts[0].thisCollider)
        {
            // This causes the animator to run the bumper animation
            animator.SetBool("Bumped", true);
            bumperSound.Play();
            foreach (ContactPoint contact in collision.contacts)
            {
                contact.otherCollider.attachedRigidbody.AddForce((-1 * contact.normal) * bumperForce, ForceMode.Impulse);
            }
        }
    }

    // Returns bumper back to idle state
    private void OnCollisionExit(Collision other)
    {
        animator.SetBool("Bumped", false);
    }

}
