using UnityEngine;

public class GameManager : MonoBehaviour
{
    // References
    public PathManager pathManager;
    public GameObject marblePrefab;
    // In game objects
    [HideInInspector] public GameObject marble;
    // State objects
    bool buildNewBoard = false;
    // Settings
    [Range(0.1f, 3.0f)] public float spawnNewBoardTiming = 1.0f;
    public int boardOffsetFromMarble = 30;

    /// <summary>Destroys the old board via tags.</summary>
    public void DeleteOldBoards()
    {
        GameObject[] wallTiles = GameObject.FindGameObjectsWithTag("boardObjects");
        if (wallTiles.Length > 0)
        {
            foreach (GameObject go in wallTiles)
            {
                Destroy(go);
            }
        }
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
    }

    // Ensures the marble is placed before any functions occur that rely on it's position
    void Awake()
    {
        PlaceMarble();
    }

    // TODO: Remove, testing only
    void Start()
    {
        pathManager.ConstructPathStack(GetMarblePositionOffset());
    }

}
