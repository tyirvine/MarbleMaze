using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalStaticVariables {

	public class DebugSettings : MonoBehaviour {
		// This is a way to make all the other variables in this class static
		//public static DebugSettings Instance { get; private set; }

		// What does 'Value' mean? - Ty
		public static int Value;

		/// <summary>This ensures that all game world objects share the same scale.</summary>
		public static Vector3 globalScale = new Vector3(1f, 1f, 1f);

		/// <summary>Used to activate debugging within the project.</summary>
		[Header("Debug Mode")]
		public static bool debugModeSwitch = false;

		// What does this do? - Ty
		//public void Awake() {
		//	if (Instance == null) {
		//		Instance = this;
		//		DontDestroyOnLoad(gameObject);
		//	} else {
		//		Destroy(gameObject);
		//	}
		//}

		//public void CallBuild() {

		//}
	}
}