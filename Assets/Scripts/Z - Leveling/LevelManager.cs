using System.ComponentModel;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{

    /* -------------------------------- Settings -------------------------------- */
    [Header("Settings")]
    public int startingPathLength = 3;

    // Hazards
    [Header("Starting Probability")]
    [Tooltip("The probability is out of 1000. So 200 would be the equivalent to 20%.")]
    [Range(0, 1000)] public int startingBumperProbability = 32;
    [Range(0, 1000)] public int startingLandmineProbability = 25;
    [Range(0, 1000)] public int startingSpikeProbability = 15;
    [Range(0, 1000)] public int startingLifeProbability = 15;

    // Hazard spawn rate
    [Header("Spawn Rate")]
    [Range(1, 100)] public int bumperSpawnRate = 3;
    [Range(1, 100)] public int landmineSpawnRate = 2;
    [Range(1, 100)] public int spikeSpawnRate = 1;
    [Range(1, 100)] public int lifeSpawnRate = 1;

    /* --------------------------- Referenced objects --------------------------- */
    [Header("References")]
    public ColorManager colorManager;
    [HideInInspector] public PathManager pathManager;
    [HideInInspector] public ShapeManager shapeManager;
    public TextMeshProUGUI UI_LevelCounter;

    // Grab references on Awake
    private void Awake()
    {
        // Path
        pathManager = FindObjectOfType<GameManager>().pathManager;
        pathManager.desiredPathLength = startingPathLength;

        // Shapes
        shapeManager = pathManager.GetComponent<ShapeManager>();
        shapeManager.hazardBumper.chanceToSpawn = 0;
        shapeManager.hazardLandmine.chanceToSpawn = 0;
        shapeManager.hazardSpike.chanceToSpawn = 0;
    }

    /* ------------------------------ State objects ----------------------------- */
    /// <summary>This keeps track of the current playthrough level count.</summary>
    public int currentLevel = 0;
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

    public void NewLevel(int level)
    {
        for (int i = 0; i < level; i++)
        {
            IncrementPathLength();
            IncrementHazardProbability();

            // Increments level count
            currentLevel++;
            if (currentLevel % 10 == 0) NewStage();

            // Updates the ui for the level count
            UI_UpdateLevelCounter();
        }
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
        if (UI_LevelCounter != null)
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
        // Bumper
        if (currentLevel <= 1)
        {
            shapeManager.hazardBumper.chanceToSpawn = startingBumperProbability;
            shapeManager.pickupLife.chanceToSpawn = startingLifeProbability;
        }
        else
        {
            shapeManager.hazardBumper.chanceToSpawn = CalculateSpawnRate(bumperSpawnRate, shapeManager.hazardBumper.chanceToSpawn);
            shapeManager.pickupLife.chanceToSpawn = CalculateSpawnRate(lifeSpawnRate, shapeManager.pickupLife.chanceToSpawn);
        }

        // Landmine
        if (currentLevel == 9)
            shapeManager.hazardLandmine.chanceToSpawn = startingLandmineProbability;
        else if (currentLevel >= 10)
            shapeManager.hazardLandmine.chanceToSpawn = CalculateSpawnRate(landmineSpawnRate, shapeManager.hazardLandmine.chanceToSpawn);

        // Spike
        if (currentLevel == 19)
            shapeManager.hazardSpike.chanceToSpawn = startingSpikeProbability;
        else if (currentLevel >= 20)
            shapeManager.hazardSpike.chanceToSpawn = CalculateSpawnRate(spikeSpawnRate, shapeManager.hazardSpike.chanceToSpawn);
    }

    /// <summary>This calculates how often the hazard should be spawning.</summary>
    int CalculateSpawnRate(int spawnrate, int input)
    {
        // if (currentLevel % spawnrate == 0) return input + 1;
        // else return input;
        return input + spawnrate;
    }

}
