using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class GlobalStaticVariables : MonoBehaviour {

    // This allows all the variables and functions within this class to be static when this class is used as an object!
    public static GlobalStaticVariables Instance { get; private set; }

    // Region just lets you collapse the code
    #region ============ Global Static Variables Settings Start ⤵ ================

    public int Value;
    public Vector3 GlobalScale = new Vector3(1f, 1f, 1f);

    public List<string> debugLog = new List<string>();

    [Header("Debug Options")]
    public bool debugMode;
    public bool collectFlags;

    [Header("Experimental")]
    public bool renderBoardAsSingleMesh = false;
    public bool obstacleGenerationComplete = false;
    public bool pathGenerationComplete = false;

    [Header("Quality Settings")]
    [Tooltip("not sure how this affects framrate in editor")]

    public bool vSync = false;

    [Header("Grid Size")]
    [Range(3, 100)] public int gridXSizeHalfLength = 50;
    [Range(3, 100)] public int gridZSizeHalfLength = 50;

    #endregion ============ Global Static Variables Settings End ⤴ ================

    /// <summary> Picks out random number </summary>
    public int RandomEven(int min, int max) {
        return Random.Range(min / 2, max / 2) * 2;
    }

    // This function writes to the debug log file? - Ty
    // Yep. - bubzy
    private void OnApplicationQuit() {
        Debug.Log("Quitting...");
        string fileName = Path.Combine(Application.streamingAssetsPath, "Debug.log");
        StreamWriter writer = new StreamWriter(fileName, true);
        foreach (string str in debugLog) {
            writer.WriteLine(str);
        }
        writer.Close();
        PlayerPrefs.SetInt("played", 110);
    }
    public void WriteDebug()
    {
        string fileName = Path.Combine(Application.streamingAssetsPath, "Debug.log");
        StreamWriter writer = new StreamWriter(fileName, true);
        foreach (string str in debugLog)
        {
            writer.WriteLine(str);
        }
        writer.Close();
        PlayerPrefs.SetInt("played", 110);
    }
    /// <summary>This function formats a log to the debug.log file into a [Timestamp + Log] format.</summary>
    public void DebugLogEntry(string log) {
        debugLog.Add("[" + System.DateTime.Now.TimeOfDay.ToString() + "]" + " " + log);
    }

    // Runs before start
    public void Awake() {
        // This allows the gsv to persist between scenes
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // Creates a directory for the debug log if it doesn't exist
        if (!Directory.Exists(Application.streamingAssetsPath)) {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        // What does this do? - Ty | You can delete this comment
        string fileName = Path.Combine(Application.streamingAssetsPath, "Debug.log");
        File.Delete(fileName);

        // Enables V-Sync!
        if (vSync) QualitySettings.vSyncCount = 4;

        // Creates the debug log's start timestamp
        debugLog.Add("========================================================");
        debugLog.Add("");
        debugLog.Add(System.DateTime.Now.ToString() + "  –  " + "DEBUG LOG START");
        debugLog.Add("");
        debugLog.Add("========================================================");
        debugLog.Add("");
    }
}
