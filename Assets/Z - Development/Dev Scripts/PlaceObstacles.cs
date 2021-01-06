using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObstacles : MonoBehaviour {

    public GameObject wall;
    public List<NodeObject> nodes = new List<NodeObject>();
    public bool triggered = false;

    private void AltStart() {
        nodes.AddRange(GameObject.FindGameObjectWithTag("PathManager").GetComponent<PathManager>().pathNodes);

    }

    private void Update() {
        if (GlobalStaticVariables.Instance.pathGenerationComplete && !triggered) {
            triggered = true;
            AltStart();
        }
    }

}
