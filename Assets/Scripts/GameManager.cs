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
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject p in player)
        {
            Destroy(p);
        }
        GameObject[] wallTiles = GameObject.FindGameObjectsWithTag("boardObjects");
        if (wallTiles.Length > 0)
        {
            foreach (GameObject go in wallTiles)
            {
                if (go.gameObject.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rigidBody = go.GetComponent<Rigidbody>();
                    rigidBody.useGravity = true;
                    rigidBody.isKinematic = false;
                    rigidBody.AddExplosionForce(100, Vector3.zero, 20);

                    Destroy(go,3f);
                }
            }
        }

      
      //  tempPM.PublicStart();
    }

    void OnFire()
    {
        ReloadLevel();
    }
}
