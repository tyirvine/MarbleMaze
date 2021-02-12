using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visibility : MonoBehaviour
{
    MeshRenderer mesh;
    private void Awake()
    {
        mesh = gameObject.GetComponent<MeshRenderer>();
    }
    private void OnBecameInvisible()
    {
        mesh.enabled = false;
    }
    private void OnBecameVisible()
    {
        mesh.enabled = true;
    }
}
