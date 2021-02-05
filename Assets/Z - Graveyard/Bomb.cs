using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionForce;
    public float explosionRadius;
    public void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "wallTile")
            {
                Rigidbody rigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
                rigidbody.AddExplosionForce(explosionForce, transform.position + Vector3.up, explosionRadius);

            }
        }
        Destroy(gameObject);
    }
    private void Awake()
    {
        transform.parent = GameObject.FindGameObjectWithTag("boardObjects").transform;
    }
}
