using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStaticVariables : MonoBehaviour {

	public static GlobalStaticVariables Instance { get; private set; }

	public int Value;
	public Vector3 GlobalScale;

	[Header("Debug Mode")]
	public bool debugMode;

	[Header("Grid Size")]
	[Range(3, 100)] public int gridXSizeHalfLength = 50;
	[Range(3, 100)] public int gridZSizeHalfLength = 50;

	public void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	public void callBuild() {

	}
}
