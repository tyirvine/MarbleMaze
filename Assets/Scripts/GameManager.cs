using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PathManager pathManager;
    PathManager tempPM;
    void Start()
    {
        tempPM = Instantiate(pathManager);
        tempPM.enabled = true;
    }

    public void ReloadLevel()
    {
        GameObject[] wallTiles = GameObject.FindGameObjectsWithTag("boardObjects");
        if (wallTiles.Length > 0)
        {
            foreach (GameObject go in wallTiles)
            {
                Destroy(go);
            }
        }

        if (GameObject.FindGameObjectWithTag("Player").gameObject)
            Destroy(GameObject.FindGameObjectWithTag("Player").gameObject);
        tempPM.PublicStart();
    }

    void OnFire()
    {
        ReloadLevel();
    }
}
