using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    private BoardActionControls boardActionControls;

    public float turnSpeed = 25f;

    public GameObject xAxisWheel;
    public GameObject zAxisWheel;

    private void Awake()
    {
        boardActionControls = new BoardActionControls();
    }

    private void OnEnable()
    {
        boardActionControls.Enable();
    }

    private void OnDisable()
    {
        boardActionControls.Disable();
    }

    void Start()
    {

    }

    void Update()
    {
        float movementInputX = boardActionControls.KeyboardControls.Tilt_X.ReadValue<float>();
        float movementInputZ = boardActionControls.KeyboardControls.Tilt_Z.ReadValue<float>();

        transform.Rotate(Vector3.right, movementInputX * -turnSpeed * Time.deltaTime);
        transform.Rotate(Vector3.forward, movementInputZ * -turnSpeed * Time.deltaTime);

        xAxisWheel.transform.Rotate(Vector3.forward, movementInputX * -turnSpeed * Time.deltaTime);
        zAxisWheel.transform.Rotate(Vector3.forward, movementInputZ * -turnSpeed * Time.deltaTime);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
    }
}
