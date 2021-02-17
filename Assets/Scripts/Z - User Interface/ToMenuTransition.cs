using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToMenuTransition : MonoBehaviour
{
    // References
    public Image transitionPanel;
    public Color startingColor;
    public Color targetColor;
    public float transitionTime = 1f;
    public AnimationCurve curve;
    bool engageLerp = false;
    float time = 0f;

    // Makes a transition to black then reloads the scene
    public void MakeTransition()
    {
        transitionPanel.gameObject.SetActive(true);
        engageLerp = true;
    }

    // Lerp the color
    private void Update()
    {
        if (engageLerp)
        {
            transitionPanel.color = Color.Lerp(startingColor, targetColor, curve.Evaluate(time));
            time += Time.deltaTime;

            if (time > transitionTime)
            {
                engageLerp = false;
                SceneManager.LoadScene("Game View");
            }
        }
    }
}
