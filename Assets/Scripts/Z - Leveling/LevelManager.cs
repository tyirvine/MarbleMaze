using UnityEngine;

public class LevelManager : MonoBehaviour
{


    /* ------------------------------ State objects ----------------------------- */
    /// <summary>This keeps track of the current playthrough level count.</summary>
    int currentLevel = 0;

    /* --------------------------- Persistence objects -------------------------- */

    public void IncrementCurrentLevel()
    {
        currentLevel++;
        Debug.Log("Level - " + currentLevel);
    }

}
