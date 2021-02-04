using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperHazard : MonoBehaviour
{
    // References
    public float bumperForce = 5f;
    public LayerMask layerToInteract;
    public Animator animator;

    private void OnCollisionEnter(Collision collision)
    {
        if ((layerToInteract.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            // This causes the animator to run the bumper animation
            animator.SetBool("Bumped", true);

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
