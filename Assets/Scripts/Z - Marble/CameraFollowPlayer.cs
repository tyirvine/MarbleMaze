
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowPlayer : MonoBehaviour
{
    // References
    GameObject player;

    // Settings
    [Header("Smoothness")]
    public float smoothness = 8f;

    /// <summary>Used to store settings for different transitions.</summary>
    [System.Serializable]
    public class TargetSmoothSettings
    {
        public float smoothnessToTarget = 5f;
        public AnimationCurve smoothnessToTargetCurve;
    }

    [Header("Smoothness To Target")]
    public TargetSmoothSettings menuAwayTransition = new TargetSmoothSettings();
    public TargetSmoothSettings startToTransition = new TargetSmoothSettings();

    float shakeMagnitude = 2; //how much is the camera moving by while shaking
    bool shaking = false; //is the camera shaking?
    float currentShakeTime = 0;

    /// <summary>This is the multiplier for the velocity vector position.</summary>
    [Range(0.01f, 0.5f)] public float lastPlayerPositionOffset = 0.1f;

    /// <summary>The reason we adjust the time to respawn here is because of how closely the camera is linked to the respawn process.</summary>
    public float timeToRespawn = 0.5f;

    // State objects
    [HideInInspector] public bool followPlayer = true;
    bool resetCameraFollow = false;
    CameraTarget cameraTarget = CameraTarget.Player;
    Vector3 lastPlayerPosition;
    Vector3 targetPosition;
    Vector3 startPosition;
    float time = 0f;
    TargetSmoothSettings currentTargetSettings;

    /// <summary>Simplifies the switch for following the player or smoothing to a target.</summary>
    enum CameraTarget
    {
        Player,
        Position
    }

    /// <summary>Checks to make sure a gamepad is available first.</summary>
    void SetGamepadRumble(float lowFrequency, float highFrequency)
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
    }

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
        cameraTarget = CameraTarget.Player;
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

    /// <summary>This is used to initialize the smoothing to target. It's because SmoothToTarget is called in update
    /// so it needs to be started somehow. Used for transitions.</summary>
    public void StartSmoothToTarget(Vector3 start, Vector3 target, TargetSmoothSettings targetSettings)
    {
        followPlayer = false;

        // Target position
        cameraTarget = CameraTarget.Position;
        startPosition = start;
        targetPosition = target;
        currentTargetSettings = targetSettings;
    }

    /// <summary>This actually smooths the camera to the target position.</summary>
    void SmoothToTarget()
    {
        Vector3 smoothedPosition = Vector3.Lerp(startPosition, targetPosition, currentTargetSettings.smoothnessToTargetCurve.Evaluate(time));
        transform.position = smoothedPosition;
        time += Time.deltaTime * currentTargetSettings.smoothnessToTarget;
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
        // Follow player or smooth to a different vector
        switch (cameraTarget)
        {
            case CameraTarget.Player:
                // Follow player
                if (followPlayer && player != null)
                    transform.position = player.transform.position;
                else if (resetCameraFollow)
                    StartSmoothCamera();
                else
                    EndSmoothCamera();
                break;

            case CameraTarget.Position:
                SmoothToTarget();
                break;

            default: break;
        }

        // Camera shake
        if (shaking)
        {
            transform.position += Random.insideUnitSphere * shakeMagnitude;

            if (Time.time > currentShakeTime)
            {
                SetGamepadRumble(0f, 0f);
                shaking = false;
            }
        }
    }

    /// <summary>Shakes the camera using the specified time and magnitude. Called on hazards like the landmine.</summary>
    public void CameraShake(float time, float magnitude)
    {
        currentShakeTime = Time.time + time;
        shakeMagnitude = magnitude;
        shaking = true;

        SetGamepadRumble(magnitude / 2, magnitude);
    }

}
