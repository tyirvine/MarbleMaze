using UnityEngine;

public class LevelManager : MonoBehaviour
{

    /* -------------------------------- Settings -------------------------------- */
    [Header("Settings")]
    public int startingPathLength = 3;
    // Hazards
    [Range(0, 100)] public int startingBumperProbability = 35;
    [Range(0, 100)] public int startingTreadmillProbability = 25;
    [Range(0, 100)] public int startingSpikeProbability = 15;

    /* --------------------------- Referenced objects --------------------------- */
    [Header("References")]
    public PathManager pathManager;
    public ShapeManager shapeManager;

    // Grab references on Awake
    private void Awake()
    {
        // Path
        pathManager = FindObjectOfType<GameManager>().pathManager;
        pathManager.desiredPathLength = startingPathLength;

        // Shapes
        shapeManager = pathManager.GetComponent<ShapeManager>();
        shapeManager.hazardBumper.chanceToSpawn = startingBumperProbability;
        shapeManager.hazardTreadmill.chanceToSpawn = startingTreadmillProbability;
        shapeManager.hazardSpike.chanceToSpawn = startingSpikeProbability;
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
        IncrementHazardProbability();
        // Leave this at the bottom
        currentLevel++;
    }

    /// <summary>Currently increments the path every 2 levels so by level 100 you have a path 50 units long.</summary>
    public void IncrementPathLength()
    {
        // if (currentLevel % 2 == 0)
        pathManager.desiredPathLength++;
    }

    /// <summary>Increases probability for each hazard dynamically.</summary>
    public void IncrementHazardProbability()
    {
        shapeManager.hazardBumper.chanceToSpawn++;
        shapeManager.hazardTreadmill.chanceToSpawn++;
        shapeManager.hazardSpike.chanceToSpawn++;
    }

}
