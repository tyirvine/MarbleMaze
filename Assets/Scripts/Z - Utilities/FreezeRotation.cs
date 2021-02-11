using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
    }
}
