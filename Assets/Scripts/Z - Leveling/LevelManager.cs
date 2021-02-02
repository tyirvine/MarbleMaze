using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /* --------------------------- Referenced objects --------------------------- */
    public int pathLength;
    // Hazards
    public int hazardBumperProbability;
    public int hazardSpikeProbability;
    public int hazardTreadmillProbability;

    // Grab references on Awake
    private void Awake()
    {
        pathLength = FindObjectOfType<PathManager>().desiredPathLength;
        // TODO: Get reference to hazard probability
        // hazardBumperProbability = FindObjectOfType<ShapeManager>().
    }

    /* ------------------------------ State objects ----------------------------- */
    /// <summary>This keeps track of the current playthrough level count.</summary>
    int currentLevel = 0;

    /* --------------------------- Persistence objects -------------------------- */
    // TODO: Save high score

    /* -------------------------------------------------------------------------- */
    /*                                   Methods                                  */
    /* -------------------------------------------------------------------------- */

    /// <summary>This is the heart of LevelManager. It triggers every time there's a new level.</summary>
    public void NewLevel()
    {
        IncrementPathLength();
        // Leave this at the bottom
        currentLevel++;
    }

    public void IncrementPathLength()
    {
        if (currentLevel % 2 == 0)
        {
            pathLength++;
            Debug.Log(pathLength);
        }
    }

}
