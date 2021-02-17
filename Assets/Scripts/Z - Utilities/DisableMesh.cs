
using UnityEngine;
using UnityEngine.Rendering;

public class DisableMesh : MonoBehaviour
{

    // Reference
    MeshRenderer meshRenderer;
    bool meshRendererValid = false;

    // Grab the renderer of the current object
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRendererValid = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (meshRendererValid)
        {
            if (meshRenderer.isVisible) meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            else meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        }
    }
}
