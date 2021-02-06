
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    // Settings
    public int livesToStartWith;

    // Persistence Objects
    [HideInInspector] public int livesRemaining;
    [HideInInspector] public int score;

    // TODO: Add Lives UI
    /* ---------------------------- Lives Management ---------------------------- */
    /// <summary>Optionally, you can add multiple lives.</summary>
    public void AddLife(int lives = 1)
    {
        livesRemaining += lives;
    }

    /// <summary>Automatically checks to see if the player has run out of lives.
    /// Optionally you can remove more lives than 1.</summary>
    public void RemoveLife(int lives = 1)
    {
        Debug.Log("Update UI for lives remaining :" + livesRemaining);
        livesRemaining -= 1;

        // Check if player has run out of lives
        if (livesRemaining < 0) GameOver();
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

    private void Start()
    {
        livesRemaining = livesToStartWith;
    }
}
