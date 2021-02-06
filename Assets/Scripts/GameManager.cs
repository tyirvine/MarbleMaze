﻿using UnityEngine;

public class GameManager : MonoBehaviour
{

    // In game objects
    [HideInInspector] public GameObject marble;
    [HideInInspector] public Rigidbody marbleRigidbody;
    [HideInInspector] public Vector3 boardStartPosition;
    [HideInInspector] public Transform boardPosition;
    PhysicMaterial material;

    // State objects
    [HideInInspector] public bool newBoardGenerating = true;
    bool marbleIsFallingLegally = true;
    bool marbleIsReparented = false;
    bool marbleHasDied = false;
    bool isStarted = true;

    // Settings
    [Header("Settings")]
    [Range(0.1f, 3.0f)] public float spawnNewBoardTiming = 1.0f;
    public int boardOffsetFromMarble = 30;
    public float marbleFallingSpeed = 50f;
    /// <summary>How far from the board does the marble fall before triggering a death event.</summary>
    public int fallDistanceToDeath = 10;

    // Debug Settings
    [Header("Debug Settings")]
    public bool debugMode = false;
    public int debugJumpToLevel = 0;
    int debugLevelTrack = 0;

    // References
    [Header("References")]
    public PlayerInput playerInput;
    public GameObject marblePrefab;
    public PathManager pathManager;
    public LevelManager levelManager;
    public StatsManager statsManager;

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
            marbleIsFallingLegally = false;
        }

        // Adjust marble's horizontal position
        if (marbleHorizontalPosition != boardHorizontalPosition)
        {
            Vector3 positionDifference = boardHorizontalPosition - marbleHorizontalPosition;
            marble.transform.position += positionDifference;
        }
    }

    /// <summary>Just a simple script to spawn the marble.</summary>
    public void PlaceMarble()
    {
        marble = Instantiate(marblePrefab, pathManager.gridPoints.startPointNode, Quaternion.identity);
        marbleRigidbody = marble.gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>Check to see if the marble has slipped off the side of the board.</summary>
    void CheckMarbleFallingIllegally()
    {
        // Run this check in update as its not physics heavy
        if (boardPosition != null && !marbleIsFallingLegally)
            if (marble.transform.position.y < (boardPosition.position.y - fallDistanceToDeath) && !marbleHasDied)
            {
                Debug.Log("Marble has died from heights");
                marbleHasDied = true;
            }
    }

    /// <summary>Returns the marble's position offset on the y.</summary>
    Vector3Int GetMarblePositionOffset() => Vector3Int.FloorToInt(marble.transform.position - new Vector3Int(0, boardOffsetFromMarble, 0));

    /* -------------------------------------------------------------------------- */
    /*                              New Board Methods                             */
    /* -------------------------------------------------------------------------- */
    /// <summary>Destroys the old board via tags.</summary>
    public void DeleteOldBoards()
    {
        GameObject[] wallTiles = GameObject.FindGameObjectsWithTag("boardObjects");
        if (wallTiles.Length > 0)
        {
            foreach (GameObject tile in wallTiles)
            {
                Destroy(tile);
            }
        }
    }

    /// <summary>Calls the new board method after a set number of seconds.</summary>
    public void CallForNewBoard()
    {
        // Pre-deletion ⤵︎
        marble.transform.SetParent(null);
        levelManager.NewLevel();
        marbleIsFallingLegally = true;

        // Call new board
        Invoke("NewBoard", spawnNewBoardTiming);
    }

    /// <summary>This method generates a new board and anything else that needs to happen.</summary>
    public void NewBoard()
    {
        DeleteOldBoards();
        pathManager.ConstructPathStack(GetMarblePositionOffset());
        boardStartPosition = pathManager.gridPoints.startPointNode + new Vector3(0.5f, 0f, 0.5f);

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
    }

    void FixedUpdate()
    {
        // This starts off true
        if (newBoardGenerating)
        {
            // Added to stop the board from rotating while the marble is falling
            playerInput.enabled = false;
            marbleRigidbody.AddForce(Vector3.up * marbleFallingSpeed, ForceMode.Force);
            MoveMarbleIntoBoard();
        }
        else
        {
            playerInput.enabled = true;
            ReparentMarble();
        }

        CheckMarbleFallingIllegally();
    }

    /* ------------------------------ Debug Related ----------------------------- */
    // @AlexMPester @bubzy-coding I think the entire game can run from here tbh
    void Start()
    {
        // TODO: Remove this from the production build!
        if (!debugMode)
            CallForNewBoard();
    }

    // TODO: Remove this from the production build!
    void Update()
    {
        if (isStarted)
        {
            if (debugMode && debugLevelTrack < debugJumpToLevel)
            {
                CallForNewBoard();
                debugLevelTrack++;
            }
            else
                isStarted = false;
        }
    }

}
