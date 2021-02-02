using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperHazard : MonoBehaviour
{
    public float bumperForce = 5f;
    public LayerMask layerToInteract;
    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if ((layerToInteract.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            Debug.Log("bingo");
            foreach (ContactPoint contact in collision.contacts)
            {
                contact.otherCollider.attachedRigidbody.AddForce((-1 * contact.normal) * bumperForce, ForceMode.Impulse);
            }
        }
    }
}
