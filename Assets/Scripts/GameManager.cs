using UnityEngine;

public class GameManager : MonoBehaviour
{
    // References
    public PathManager pathManager;
    public GameObject marblePrefab;
    public ColorManager colorManager;

    // In game objects
    [HideInInspector] public GameObject marble;
    [HideInInspector] public Vector3 boardStartPosition;

    Vector3 boardPosition;

    // State objects
    bool buildNewBoard = false;
    bool marbleIsFalling = false;
    private string oldWallTileTag = "oldWallTiles";

    PhysicMaterial material;

    // Settings
    [Range(0.1f, 3.0f)] public float spawnNewBoardTiming = 1.0f;
    public int boardOffsetFromMarble = 30;
    public float rateOfMarbleMovement = 1.0f;
    public float marbleFallingSpeed = 50f;
    /// <summary>Retag old boards.</summary>
    public void RetagOldBoard()
    {
        GameObject[] oldWallTiles = GameObject.FindGameObjectsWithTag("boardObjects");
        GameObject oldWallTileGroup = GameObject.Find("wallTiles Group");
        if (oldWallTiles.Length > 0)
        {
            oldWallTileGroup.tag = oldWallTileTag;
            foreach (GameObject tile in oldWallTiles)
            {
                tile.tag = oldWallTileTag;
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
        boardPosition = GameObject.FindGameObjectWithTag("boardObjects").transform.position;
    }

    /// <summary>Find the board's finish hole collider.</summary>
    public bool FindFinishHoleCollider()
    {
        return pathManager.GetComponent<BuildBoard>().pathFinishHole.GetComponent<BoxCollider>();
    }

    /// <summary>Just a simple script to spawn the marble.</summary>
    public void PlaceMarble()
    {
        marble = Instantiate(marblePrefab, pathManager.gridPoints.startPointNode, Quaternion.identity);
    }

    /// <summary>Returns the marble's position offset on the y.</summary>
    Vector3Int GetMarblePositionOffset() => Vector3Int.FloorToInt(marble.transform.position - new Vector3Int(0, boardOffsetFromMarble, 0));

    /* -------------------------------------------------------------------------- */
    /*                              New Board Methods                             */
    /* -------------------------------------------------------------------------- */

    /// <summary>Calls the new board method after a set number of seconds.</summary>
    public void CallForNewBoard()
    {
        Invoke("NewBoard", spawnNewBoardTiming);
    }

    /// <summary>This method generates a new board and anything else that needs to happen.</summary>
    public void NewBoard()
    {
        DeleteOldBoards();
        pathManager.ConstructPathStack(GetMarblePositionOffset());
        boardStartPosition = pathManager.gridPoints.startPointNode;

        // Guide marble to new board start position
        marbleIsFalling = true;

        // Add any code below that needs to be execute upon starting a new level

        // Change colour
        colorManager.changeColor = true;
    }

    // Ensures the marble is placed before any functions occur that rely on it's position
    void Awake()
    {
        PlaceMarble();
    }

    void FixedUpdate()
    {
        if (marbleIsFalling)
        {
            material.bounceCombine = PhysicMaterialCombine.Minimum;
            marble.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, -marbleFallingSpeed, 0);
            //marble.gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 10;

            MoveMarbleIntoBoard();
        }

        if (marble.transform.position.y <= boardPosition.y + 1f)
        {
            material.bounceCombine = PhysicMaterialCombine.Average;
            //   marble.gameObject.GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
        }

    }

    // TODO: Remove, testing only
    void Start()
    {
        pathManager.ConstructPathStack(GetMarblePositionOffset());
        material = marble.GetComponent<Collider>().material;
        //get the current ACTUAL board position for calculating marble physics behaviour
        boardPosition = GameObject.FindGameObjectWithTag("boardObjects").transform.position;
    }

}
