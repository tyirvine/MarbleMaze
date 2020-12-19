using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GlobalStaticVariables : MonoBehaviour {

	public static GlobalStaticVariables Instance { get; private set; }

	public int Value;
	public Vector3 GlobalScale = new Vector3(1f, 1f, 1f);

	public List<string> debugLog = new List<string>();

	[Header("Debug Options")]
	public bool debugMode;
	public bool collectFlags;

	[Header("Experimental")]
	public bool renderBoardAsSingleMesh = false;
	public bool obstacleGenerationComplete = false;

	[Header("Quality Settings")]
	[Tooltip("not sure how this affects framrate in editor")]

	public bool vSync = false;

	[Header("Grid Size")]
	[Range(3, 100)] public int gridXSizeHalfLength = 50;
	[Range(3, 100)] public int gridZSizeHalfLength = 50;




	// This function fills in the debug log file? - Ty   Yep. - bubzy
	private void OnApplicationQuit() {
		Debug.Log("Quitting");
		string fileName = Path.Combine(Application.streamingAssetsPath, "Debug.log");
		StreamWriter writer = new StreamWriter(fileName, true);
		foreach (string str in debugLog) {
			writer.WriteLine(str);
		}
		writer.Close();
	}


	// Runs before start
	public void Awake() {
	
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	void Start() {
		if(!Directory.Exists(Application.streamingAssetsPath))
        {
			Directory.CreateDirectory(Application.streamingAssetsPath);
				
        }
			string fileName = Path.Combine(Application.streamingAssetsPath, "Debug.log");
			File.Delete(fileName);
		if (vSync) {
			QualitySettings.vSyncCount = 4;
		}
		debugLog.Add("Debug Log for "+System.DateTime.Now.ToString());

	}
}
