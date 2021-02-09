
using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    // Settings
    public int livesToStartWith;

    // Persistence Objects
    [HideInInspector] public int livesRemaining;
    [HideInInspector] public int score;

    // References
    [Header("References")]
    public TextMeshProUGUI UI_LivesCounter;

    // TODO: Add Lives UI
    /* ---------------------------- Lives Management ---------------------------- */
    /// <summary>The root method for adjusting the life count.</summary>
    public void AdjustLifeCount(int lives = 1)
    {
        // Adjust count
        livesRemaining += lives;

        // Adjust UI
        string livesFormatted = "Lives x " + livesRemaining;
        UI_LivesCounter.text = livesFormatted;
    }
    /// <summary>Optionally, you can add multiple lives.</summary>
    public void AddLife(int lives = 1)
    {
        AdjustLifeCount(lives);
    }

    /// <summary>Automatically checks to see if the player has run out of lives.
    /// Optionally you can remove more lives than 1.</summary>
    public void RemoveLife(int lives = -1)
    {
        AdjustLifeCount(lives);

        // Check if player has run out of lives
        if (livesRemaining < 0) GameOver();
        else { } // Run respawn
    }

    /// <summary>This runs the entire game over process.</summary>
    public void GameOver()
    {
        Debug.Log("Heres where we trigger the gameover stuff");
    }

    /* ---------------------------- Score Management ---------------------------- */
    /// <summary>Adds 1 point by default but you can add more points.</summary>
    public void AddToScore(int score = 1)
    {
        this.score += score;
    }

    /* -------------------------------------------------------------------------- */
    /*                                    Main                                    */
    /* -------------------------------------------------------------------------- */

    // Start the player with a designated number of lives
    private void Awake()
    {
        AddLife(livesToStartWith);
    }
}
