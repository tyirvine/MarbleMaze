using UnityEngine;

public class GameManager : MonoBehaviour
{

    // In game objects
    [HideInInspector] public GameObject marble;
    [HideInInspector] public Rigidbody marbleRigidbody;
    [HideInInspector] public Vector3 boardStartPosition;

    // Vector3 boardPosition;
    PhysicMaterial material;

    // State objects
    bool buildNewBoard = false;
    bool marbleIsFalling = true;
    bool marbleIsReparented = false;
    private string oldWallTileTag = "oldWallTiles";

    // Settings
    [Range(0.1f, 3.0f)] public float spawnNewBoardTiming = 1.0f;
    public int boardOffsetFromMarble = 30;
    public float marbleFallingSpeed = 50f;

    // References
    [Header("References")]
    public PathManager pathManager;
    public GameObject marblePrefab;
    public LevelManager levelManager;

    /* -------------------------------------------------------------------------- */
    /*                                   Methods                                  */
    /* -------------------------------------------------------------------------- */

    /// <summary>Parents the marble. This eliminates stutter during board movement.</summary>
    public void ReparentMarble()
    {
        if (!marbleIsReparented)
        {
            GameObject currentBoard = GameObject.FindGameObjectWithTag("boardObjects");
            if (currentBoard != null)
            {
                marble.transform.parent = currentBoard.transform;
                marbleIsReparented = true;
            }
        }
    }

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

    /// <summary>This function is designed to move the marble into the board's position.</summary>
    public void MoveMarbleIntoBoard()
    {

        Vector3 marbleHorizontalPosition = new Vector3(marble.transform.position.x, 0, marble.transform.position.z);
        Vector3 boardHorizontalPosition = new Vector3(boardStartPosition.x, 0, boardStartPosition.z);

        // Disable if the marble is near the board
        int verticalTriggerPadding = 7;
        if (marble.transform.position.y < boardStartPosition.y + verticalTriggerPadding && marble.transform.position.y > boardStartPosition.y - verticalTriggerPadding)
        {
            marbleIsFalling = false;
        }

        // Adjust marble's horizontal position
        if (marbleHorizontalPosition != boardHorizontalPosition)
        {
            Vector3 positionDifference = boardHorizontalPosition - marbleHorizontalPosition;
            marble.transform.position += positionDifference;
        }

        //get the current ACTUAL board position for calculating marble physics behaviour
        // boardPosition = GameObject.FindGameObjectWithTag("boardObjects").transform.position;
    }

    /// <summary>Just a simple script to spawn the marble.</summary>
    public void PlaceMarble()
    {
        marble = Instantiate(marblePrefab, pathManager.gridPoints.startPointNode, Quaternion.identity);
        marbleRigidbody = marble.gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>Returns the marble's position offset on the y.</summary>
    Vector3Int GetMarblePositionOffset() => Vector3Int.FloorToInt(marble.transform.position - new Vector3Int(0, boardOffsetFromMarble, 0));

    /* -------------------------------------------------------------------------- */
    /*                              New Board Methods                             */
    /* -------------------------------------------------------------------------- */

    /// <summary>Calls the new board method after a set number of seconds.</summary>
    public void CallForNewBoard()
    {
        // Pre-deletion ⤵︎
        marble.transform.SetParent(null);
        levelManager.NewLevel();

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
        marbleIsFalling = true;
        marbleIsReparented = false;

        // Add any code below that needs to be execute upon starting a new level ⤵︎

    }

    // Ensures the marble is placed before any functions occur that rely on it's position
    void Awake()
    {
        PlaceMarble();
    }

    void FixedUpdate()
    {
        // This starts off true
        if (marbleIsFalling)
        {
            marbleRigidbody.AddForce(Vector3.up * marbleFallingSpeed, ForceMode.Force);
            // material.bounceCombine = PhysicMaterialCombine.Minimum;
            // marble.gameObject.GetComponent<Rigidbody>().velocity += (new Vector3(0, -marbleFallingSpeed, 0) * Time.fixedDeltaTime);
            //marble.gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 10;
            MoveMarbleIntoBoard();
        }
        else
            ReparentMarble();
        // if (marble.transform.position.y <= boardPosition.y + 1f)
        // {
        //     material.bounceCombine = PhysicMaterialCombine.Average;
        //     //   marble.gameObject.GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
        // }
    }

    // @AlexMPester @bubzy-coding I think the entire game can run from here tbh
    void Start()
    {
        CallForNewBoard();
        // pathManager.ConstructPathStack(GetMarblePositionOffset());
        // material = marble.GetComponent<Collider>().material;
        //get the current ACTUAL board position for calculating marble physics behaviour
        // boardPosition = GameObject.FindGameObjectWithTag("boardObjects").transform.position;
    }

}
