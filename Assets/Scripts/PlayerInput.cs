
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector]
    public GameObject boardObjects;

    public float boardClamp = 20f;
    Vector2 boardMovement = new Vector2(0, 0);
    public float moveSpeed = 25f;

    GameManager gameManager;

    private void Awake()
    {
        //used as a reference for the marbles position
        gameManager = gameObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (boardObjects)
        {
            Vector3 rotEulers = boardObjects.transform.rotation.eulerAngles;
            rotEulers.x = (rotEulers.x <= 180 ? rotEulers.x : -(360 - rotEulers.x));
            rotEulers.x = Mathf.Clamp(rotEulers.x, -boardClamp, boardClamp);
            rotEulers.z = (rotEulers.z <= 180 ? rotEulers.z : -(360 - rotEulers.z));
            rotEulers.z = Mathf.Clamp(rotEulers.z, -boardClamp, boardClamp);
            rotEulers.y = 0;
            boardObjects.transform.eulerAngles = rotEulers;
            boardObjects.transform.RotateAround(gameManager.marble.transform.position, Vector3.right, boardMovement.x * moveSpeed * Time.fixedDeltaTime);
            boardObjects.transform.RotateAround(gameManager.marble.transform.position, Vector3.forward, -boardMovement.y * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            boardObjects = GameObject.FindGameObjectWithTag("boardObjects");
        }
    }

}
