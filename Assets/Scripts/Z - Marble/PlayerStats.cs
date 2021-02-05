using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public int livesRemaining;
    public int score;

    public PlayerStats(int lives)
    {
        livesRemaining = lives;
    }

    public void AddLife(int lives)
    {
        livesRemaining += lives;
    }

    public void RemoveLife(int lives)
    {
        livesRemaining -= lives;
    }

    public void AdjustScore(int _score)
    {
        score += _score;
    }


}
