using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    // References
    GameObject player;

    // Settings
    public float smoothness = 8f;

    /// <summary>This is the multiplier for the velocity vector position.</summary>
    [Range(0.01f, 0.5f)] public float lastPlayerPositionOffset = 0.1f;

    /// <summary>The reason we adjust the time to respawn here is because of how closely the camera is linked to the respawn process.</summary>
    public float timeToRespawn = 0.5f;

    // State objects
    [HideInInspector] public bool followPlayer = true;
    bool resetCameraFollow = false;
    Vector3 lastPlayerPosition;

    /// <summary>Resets all state objects.</summary>
    void CameraInit()
    {
        followPlayer = true;
        resetCameraFollow = false;
    }

    /// <summary>Used to stop following the player and trigger EndSmoothCamera().</summary>
    public void StopFollowingPlayer()
    {
        followPlayer = false;

        // Calculate how far ahead to move the camera
        Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
        lastPlayerPosition = player.transform.position + (velocity.normalized * velocity.magnitude * lastPlayerPositionOffset);
    }

    /// <summary>Used to reset camera following.</summary>
    public void StartFollowingPlayer()
    {
        resetCameraFollow = true;
    }

    /// <summary>This snaps the camera to the desired position.</summary>
    void StartSmoothCamera()
    {
        // We increase the smoothness so it returns to the original position faster.
        // Without this you can see the jump between the smoothing and fixed camera movement.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, player.transform.position, (smoothness * 1.5f) * Time.deltaTime);
        transform.position = smoothedPosition;
        Invoke("CameraInit", timeToRespawn);
    }

    /// <summary>This allows the camera to follow smoothly to the player's position.</summary>
    void EndSmoothCamera()
    {
        // Calculate camera smoothing
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, lastPlayerPosition, smoothness * Time.deltaTime);

        // Assign the smoothed position
        transform.position = smoothedPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<GameManager>().marble;
    }

    // Update is called once per frame : Dictates whether or not the camera movement is being smoothed
    void LateUpdate()
    {
        if (followPlayer)
            transform.position = player.transform.position;
        else if (resetCameraFollow)
            StartSmoothCamera();
        else
            EndSmoothCamera();

    }
}
