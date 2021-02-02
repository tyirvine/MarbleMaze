using UnityEngine;

public class LevelManager : MonoBehaviour
{

    /* -------------------------------- Settings -------------------------------- */
    [Header("Settings")]
    public int startingPathLength = 3;

    /* --------------------------- Referenced objects --------------------------- */
    [Header("References")]
    public PathManager pathManager;
    // Hazards
    public int hazardBumperProbability;
    public int hazardSpikeProbability;
    public int hazardTreadmillProbability;

    // Grab references on Awake
    private void Awake()
    {
        // Path
        pathManager = FindObjectOfType<GameManager>().pathManager;
        pathManager.desiredPathLength = startingPathLength;

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

    /// <summary>Currently increments the path every 2 levels so by level 100 you have a path 50 units long.</summary>
    public void IncrementPathLength()
    {
        if (currentLevel % 2 == 0)
            pathManager.desiredPathLength++;
    }

}
