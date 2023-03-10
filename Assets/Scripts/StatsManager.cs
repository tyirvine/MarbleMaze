
using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    // Settings
    public int livesToStartWith;

    // Persistence Objects
    [HideInInspector] public int livesRemaining;
    [HideInInspector] public int score;

    // References
    [Header("References")]
    public CameraFollowPlayer cameraRig;
    public GameManager gameManager;
    public TextMeshProUGUI UI_LivesCounter;
    public PlayerInput playerInput;
    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject board;
    [HideInInspector] public UIManager uiManager;

    /* ---------------------------- Lives Management ---------------------------- */
    /// <summary>The root method for adjusting the life count.</summary>
    public void AdjustLifeCount(int lives = 1)
    {
        // Adjust count
        livesRemaining += lives;

        // Adjust UI
        string livesFormatted = "Lives x " + livesRemaining;

        if (UI_LivesCounter != null && livesRemaining >= 0)
            UI_LivesCounter.text = livesFormatted;
    }
    /// <summary>Optionally, you can add multiple lives.</summary>
    public void AddLife(int lives = 1)
    {
        AdjustLifeCount(lives);
    }

    /// <summary>Automatically checks to see if the player has run out of lives.
    /// Optionally you can remove more lives than 1.</summary>
    public void RemoveLife(int lives = -1)
    {
        AdjustLifeCount(lives);

        // Check if player has run out of lives
        if (livesRemaining < 0)
            GameOver();
        else
            ActivatePlayerRespawn();
    }

    /// <summary>This runs immediately after a life is removed.</summary>
    void ActivatePlayerRespawn()
    {
        cameraRig.StopFollowingPlayer();
        Invoke("RespawnPlayer", cameraRig.timeToRespawn);
    }

    /// <summary>This is the core functionallity of respawning the player.</summary>
    void RespawnPlayer()
    {
        // Reset board rotation - current - this does work
        board = GameObject.FindGameObjectWithTag("boardObjects");
        board.transform.position = gameManager.pathManager.gridPoints.originalBoardPosition;
        board.transform.rotation = gameManager.pathManager.gridPoints.originalBoardRotation;

        // Reset player's position and velocity
        player.transform.position = gameManager.pathManager.gridPoints.startPointNodeAdjusted + new Vector3Int(0, 1, 0);
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;

        // Reset player's condition
        player.GetComponent<MarbleBehaviour>().RespawnSequence();

        // Unlatch controls
        Invoke(nameof(UnlatchControls), 0.1f);

        // Reset camera
        cameraRig.StartFollowingPlayer();
    }

    /// <summary>Used to delay the unlatching of controls. There's a bug if there's no delay taken</summary>
    void UnlatchControls()
    {
        playerInput.PauseControls(false);
    }

    /// <summary>This runs the entire game over process.</summary>
    public void GameOver()
    {
        // Stop following the player
        cameraRig.StopFollowingPlayer();

        // Pull up game over ui
        if (uiManager != null)
        {
            uiManager.GameoverMenu(true);
        }
    }

    /* ---------------------------- Score Management ---------------------------- */
    /// <summary>Adds 1 point by default but you can add more points.</summary>
    public void AddToScore(int score = 1)
    {
        this.score += score;
    }

    /* -------------------------------------------------------------------------- */
    /*                                    Main                                    */
    /* -------------------------------------------------------------------------- */

    // Start the player with a designated number of lives
    // ! DO NOT CHANGE TO AWAKE, IT CAN ONLY GRAB REFERENCE ON START !
    private void Start()
    {
        AddLife(livesToStartWith);
        player = GameObject.FindObjectOfType<GameManager>().marble;
        uiManager = GameObject.FindObjectOfType<UIManager>();
    }

}
