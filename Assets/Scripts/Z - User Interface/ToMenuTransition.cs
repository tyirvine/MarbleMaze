using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToMenuTransition : MonoBehaviour
{
    // References
    public float transitionTime = 1f;
    public float cameraYShift = 30f;
    [HideInInspector] public CameraFollowPlayer cameraManager;
    [HideInInspector] public ColorManager colorManager;
    [HideInInspector] public UIManager uiManager;

    // Grab references
    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraFollowPlayer>();
        colorManager = FindObjectOfType<ColorManager>();
        uiManager = FindObjectOfType<UIManager>();
    }

    // Makes a transition to black then reloads the scene
    public void MakeTransition()
    {
        // Unpause game
        Time.timeScale = 1f;

        // Initiate end transition
        IEnumerator coroutine = EndTransition();
        StartCoroutine(coroutine);

        // Shift camera
        Vector3 cameraStart = cameraManager.gameObject.transform.position;
        Vector3 cameraTarget = cameraStart - new Vector3(0f, cameraYShift, 0f);
        cameraManager.StartSmoothToTarget(cameraStart, cameraTarget);

        // Shift colour
        colorManager.changeColor = ColorManager.ChangeColor.Revert;

        // Close menu
        uiManager.GameoverMenu(false);
        uiManager.PauseMenu(false);
    }

    // This is the coroutine that actually runs the transition
    IEnumerator EndTransition()
    {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("Game View");
    }
}
