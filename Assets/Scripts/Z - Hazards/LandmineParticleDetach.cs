
using UnityEngine;

public class DetachFromPrefabOnAwake : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.SetParent(null);
    }
}
