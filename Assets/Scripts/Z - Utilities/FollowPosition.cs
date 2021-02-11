using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    // Position to follow
    public GameObject positionToFollow;
    public Vector3 offset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = positionToFollow.transform.position + offset;
    }
}
