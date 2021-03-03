
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // References
    [HideInInspector] public GameObject boardObjects;
    [HideInInspector] public UIManager uiManager;

    // State objects
    public float boardClamp = 20f;
    Vector2 boardMovement = new Vector2(0, 0);
    Vector2 inputValue;
    public float moveSpeed = 25f;
    bool pauseState = false;
    bool isStart = true;

    GameManager gameManager;
    MarbleBehaviour marbleBehaviour;

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        uiManager = GameObject.FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        marbleBehaviour = gameManager.marble.GetComponent<MarbleBehaviour>();
    }

    /// <summary>A script to pause the controls when needed.</summary>
    public void PauseControls(bool pause)
    {
        pauseState = pause;
        boardMovement.x = 0f;
        boardMovement.y = 0f;
    }

    // Used to move the board
    void OnLook(InputValue _value)
    {

        // Assign input value
        inputValue = _value.Get<Vector2>();

        if (!pauseState)
        {
            if (isStart)
            {
                // Hide user interface
                gameManager.ConfigureForRuntime();
                uiManager.StartMenu(false);
                uiManager.StatsMenu(true);
                isStart = false;
            }
        }
    }

    /// <summary>Pauses the whole game using time.</summary>
    void OnPause()
    {
        if (!uiManager.pauseMenu.state && !uiManager.startMenu.state)
        {
            uiManager.pauseMenu.state = true;
            uiManager.PauseMenu(uiManager.pauseMenu.state);
        }
        else
        {
            uiManager.pauseMenu.state = false;
            uiManager.PauseMenu(uiManager.pauseMenu.state);
        }
    }

    // Causes player to jump
    void OnJump()
    {
        marbleBehaviour.Jump();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (boardObjects && !pauseState && inputValue.magnitude >= 0f)
        {
            // Setup input influence on board movement
            boardMovement.x = inputValue.y;
            boardMovement.y = inputValue.x;
            if (GlobalStaticVariables.Instance.invertX) { boardMovement.y = -boardMovement.y; }
            if (GlobalStaticVariables.Instance.invertY) { boardMovement.x = -boardMovement.x; }

            // Calculate board rotation
            Vector3 rotEulers = boardObjects.transform.rotation.eulerAngles;
            rotEulers.x = (rotEulers.x <= 180 ? rotEulers.x : -(360 - rotEulers.x));
            rotEulers.x = Mathf.Clamp(rotEulers.x, -boardClamp, boardClamp);
            rotEulers.z = (rotEulers.z <= 180 ? rotEulers.z : -(360 - rotEulers.z));
            rotEulers.z = Mathf.Clamp(rotEulers.z, -boardClamp, boardClamp);
            rotEulers.y = 0;
            boardObjects.transform.eulerAngles = rotEulers;
            boardObjects.transform.RotateAround(gameManager.marble.transform.position, Vector3.right, boardMovement.x * moveSpeed * Time.fixedDeltaTime);
            boardObjects.transform.RotateAround(gameManager.marble.transform.position, Vector3.forward, -boardMovement.y * moveSpeed * Time.fixedDeltaTime);

            // Lock board's position
            boardObjects.transform.position = gameManager.pathManager.gridPoints.originalBoardPosition;
        }
        else if (boardObjects == null)
        {
            boardObjects = GameObject.FindGameObjectWithTag("boardObjects");
        }
    }

}
