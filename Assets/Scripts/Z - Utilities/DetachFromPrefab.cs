
using UnityEngine;

public class DetachFromPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.SetParent(null);
    }
}
