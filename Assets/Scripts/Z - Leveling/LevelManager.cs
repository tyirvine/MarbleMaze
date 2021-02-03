using UnityEngine;
using TMPro;

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
    public ColorManager colorManager;
    [HideInInspector] public PathManager pathManager;
    [HideInInspector] public ShapeManager shapeManager;
    [HideInInspector] public TextMeshProUGUI UI_LevelCounter;

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

        // UI
        UI_LevelCounter = FindObjectOfType<TextMeshProUGUI>();
    }

    /* ------------------------------ State objects ----------------------------- */
    /// <summary>This keeps track of the current playthrough level count.</summary>
    int currentLevel = 0;
    int currentStage = 1;

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

        // Increments level count
        currentLevel++;
        if (currentLevel % 10 == 0) NewStage();

        // Updates the ui for the level count
        UI_UpdateLevelCounter();
    }

    /// <summary>This is sort of like an extension to new level. This just runs code after entering a new stage.</summary>
    public void NewStage()
    {
        currentStage++;

        // Change colours
        colorManager.changeColor = true;
    }

    /* -------------------------------------------------------------------------- */
    /*                               Level Updaters                               */
    /* -------------------------------------------------------------------------- */

    /// <summary>Updates the level counter user interface!</summary>
    public void UI_UpdateLevelCounter()
    {
        // Format text first
        string stage = "S" + currentStage;
        string level = "L" + currentLevel;
        string counterFormatted = stage + " - " + level;

        // Update text
        UI_LevelCounter.text = counterFormatted;
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
