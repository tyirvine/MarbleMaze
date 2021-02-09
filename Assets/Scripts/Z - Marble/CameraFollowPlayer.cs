using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    // References
    GameObject player;

    // Settings
    public float smoothness = 8f;
    public float lastPlayerPositionOffset = 6f;

    // State objects
    [HideInInspector] public bool followPlayer = true;
    [HideInInspector] public bool stopFollowingPlayer = false;
    bool resetCameraFollow = false;
    Vector3 lastPlayerPosition;

    /// <summary>Resets all state objects.</summary>
    void CameraInit()
    {
        followPlayer = true;
        stopFollowingPlayer = false;
        resetCameraFollow = false;
    }

    /// <summary>Used to stop following the player and trigger EndSmoothCamera().</summary>
    public void StopFollowingPlayer()
    {
        followPlayer = false;
        lastPlayerPosition = player.transform.position + (player.GetComponent<Rigidbody>().velocity.normalized * lastPlayerPositionOffset);
    }

    /// <summary>Used to reset camera following.</summary>
    public void StartFollowingPlayer()
    {
        resetCameraFollow = true;
        player = GameObject.FindObjectOfType<GameManager>().marble;
    }

    /// <summary>This snaps the camera to the desired position.</summary>
    void StartSmoothCamera()
    {
        // Run as long as the camera's position hasn't reached the player's
        if (transform.position != player.transform.position)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, player.transform.position, smoothness * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        // Then reset all values
        else
        {
            CameraInit();
        }
    }

    /// <summary>This allows the camera to follow smoothly to the player's position.</summary>
    void EndSmoothCamera()
    {
        // Calculate camera smoothing
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, lastPlayerPosition, smoothness * Time.deltaTime);

        // Assign the smoothed position
        transform.position = smoothedPosition;
    }

    /// <summary>Just used to delay the end to following the player.</summary>
    void EndPlayerFollow() => stopFollowingPlayer = true;

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
