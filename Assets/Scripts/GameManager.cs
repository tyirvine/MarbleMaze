using UnityEngine;
[System.Serializable]
public class GameManager : MonoBehaviour
{

    // In game objects
    [HideInInspector] public GameObject marble;
    [HideInInspector] public Rigidbody marbleRigidbody;
    [HideInInspector] public Vector3 boardStartPosition;
    [HideInInspector] public Transform boardPosition;
    PhysicMaterial material;
    bool start = true;

    // State objects
    [HideInInspector] public bool newBoardGenerating = true;
    bool marbleIsReparented = false;
    [HideInInspector] public GameObject oldBoard;

    // Settings
    [Header("Settings")]
    [HideInInspector] public float spawnNewBoardTiming = 1.0f;
    [Range(0.0f, 3.0f)] public float startingNewBoardTiming = 0f;
    [Range(0.0f, 3.0f)] public float runtimeNewBoardTiming = 1f;

    // Offset
    [HideInInspector] public int boardOffsetFromMarble = 30;
    public int startingBoardOffset = 0;
    public int runtimeBoardOffset = 30;
    public float marbleFallingSpeed = 50f;

    // Debug Settings
    [Header("Debug Settings")]
    public bool debugMode = false;
    public int debugJumpToLevel;

    // High Score
    public int currentScore = 0;

    // References
    [Header("References")]
    public PlayerInput playerInput;
    public GameObject marblePrefab;
    public PathManager pathManager;
    public LevelManager levelManager;
    public StatsManager statsManager;
    public CameraFollowPlayer cameraManager;
    public UIManager uiManager;

    /* -------------------------------------------------------------------------- */
    /*                           Marble Related Methods                           */
    /* -------------------------------------------------------------------------- */
    /// <summary>Parents the marble. This eliminates stutter during board movement.</summary>
    public void ReparentMarble()
    {
        if (!marbleIsReparented)
        {
            GameObject currentBoard = GameObject.FindGameObjectWithTag("boardObjects");

            if (currentBoard != null)
            {
                // Allow this to be accessible from other scripts(marble in particular)
                boardPosition = currentBoard.transform;
                marble.transform.parent = currentBoard.transform;
                marbleIsReparented = true;
            }
        }
    }

    /// <summary>This function is designed to move the marble into the board's position.</summary>
    public void MoveMarbleIntoBoard()
    {
        Vector3 marbleHorizontalPosition = new Vector3(marble.transform.position.x, 0, marble.transform.position.z);
        Vector3 boardHorizontalPosition = new Vector3(boardStartPosition.x, 0, boardStartPosition.z);

        // Disable if the marble is near the board
        int verticalTriggerPadding = 5;
        if (marble.transform.position.y < boardStartPosition.y + verticalTriggerPadding && marble.transform.position.y > boardStartPosition.y - verticalTriggerPadding)
        {
            newBoardGenerating = false;
            start = false;
        }

        // Move marble's horizontal position. Runs until the marble is in good alignment.
        if ((marbleHorizontalPosition - boardHorizontalPosition).magnitude >= 0.1)
        {
            Vector3 positionDifference = boardHorizontalPosition - marbleHorizontalPosition;
            marble.transform.position += positionDifference;
        }
    }

    /// <summary>Just a simple script to spawn the marble.</summary>
    public void PlaceMarble()
    {
        marble = Instantiate(marblePrefab, Vector3.zero, Quaternion.identity);
        marbleRigidbody = marble.gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>Returns the marble's position offset on the y.</summary>
    Vector3Int GetMarblePositionOffset() => Vector3Int.FloorToInt(marble.transform.position - new Vector3Int(0, boardOffsetFromMarble, 0));

    /* -------------------------------------------------------------------------- */
    /*                              New Board Methods                             */
    /* -------------------------------------------------------------------------- */
    /// <summary>Destroys the old board via tags.</summary>
    public void DeleteOldBoards()
    {
        GameObject[] boards = GameObject.FindGameObjectsWithTag("boardObjects");
        if (boards != null)
        {
            foreach (GameObject board in boards)
            {
                board.transform.DetachChildren();
                Destroy(board);
            }
        }
    }

    /// <summary>Calls the new board method after a set number of seconds.</summary>
    public void CallForNewBoard()
    {
        // Calls the board. This has been made into a nested function for debugging
        void BoardCall()
        {
            // Pre-deletion ⤵︎
            marble.transform.SetParent(null);
            levelManager.NewLevel();

            // Delete death catch so we don't get false deaths on level completion
            GameObject deathCatch = pathManager.GetComponent<BuildBoard>().deathCatch;
            if (deathCatch != null) Destroy(deathCatch.gameObject);

            // Call new board
            Invoke(nameof(NewBoard), spawnNewBoardTiming);

            currentScore = levelManager.currentLevel;
        }

        // ! Remove from production
        // Each time a level is beat, if in debug mode, it'll jump the next x number of levels
        if (debugMode)
            for (int level = 0; level < debugJumpToLevel; level++)
                BoardCall();
        else
            BoardCall();
    }

    /// <summary>This method generates a new board and anything else that needs to happen.</summary>
    public void NewBoard()
    {
        DeleteOldBoards();
        pathManager.ConstructPathStack(GetMarblePositionOffset());
        boardStartPosition = pathManager.gridPoints.startPointNodeAdjusted;

        // Guide marble to new board start position
        newBoardGenerating = true;
        marbleIsReparented = false;

        // Add any code below that needs to be execute upon starting a new level ⤵︎

    }

    /* -------------------------------------------------------------------------- */
    /*                                    Main                                    */
    /* -------------------------------------------------------------------------- */

    // Ensures the marble is placed before any functions occur that rely on it's position
    void Awake()
    {
        // Debug Mode
        if (debugMode) Debug.LogWarning("Level Manager Debug Mode Enabled");

        // Marble
        PlaceMarble();
        Debug.Log(PlayerPrefs.GetInt("played"));
    }

    void FixedUpdate()
    {
        // This starts off true
        if (newBoardGenerating && !start)
        {
            // Added to stop the board from rotating while the marble is falling
            // playerInput.enabled = false;
            marbleRigidbody.AddForce(Vector3.up * marbleFallingSpeed, ForceMode.Force);
            MoveMarbleIntoBoard();
        }
        else if (start)
        {
            // playerInput.enabled = false;
            marbleRigidbody.AddForce(Vector3.up * (marbleFallingSpeed / 2f), ForceMode.Force);
            MoveMarbleIntoBoard();
        }
        else
        {

            ReparentMarble();
        }
    }

    /* ------------------------------ Debug Related ----------------------------- */
    // @AlexMPester @bubzy-coding I think the entire game can run from here tbh
    void Start()
    {
        // Initial board creation
        boardOffsetFromMarble = startingBoardOffset;
        spawnNewBoardTiming = startingNewBoardTiming;

        // Setup User Interface
        uiManager.StartMenu(true);

        // Camera setup
        GameObject temp_marble = FindObjectOfType<MarbleBehaviour>().gameObject;
        Vector3 cameraStart = cameraManager.gameObject.transform.position;
        cameraManager.StartSmoothToTarget(cameraStart, temp_marble, cameraManager.startToTransition);

        CallForNewBoard();

        // Configure for runtime without player action
        // Invoke(nameof(ConfigureForRuntime), 3f);
    }

    /// <summary>Used to configure board generation for runtime.
    /// This is called then the player first moves.</summary>
    public void ConfigureForRuntime()
    {
        cameraManager.StartFollowingPlayer();
        boardOffsetFromMarble = runtimeBoardOffset;
        spawnNewBoardTiming = runtimeNewBoardTiming;
    }
}
