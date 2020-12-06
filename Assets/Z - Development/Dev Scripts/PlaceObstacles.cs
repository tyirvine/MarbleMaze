using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObstacles : MonoBehaviour
{
    //the tile to be used for the obstacle.
    public GameObject obstacleObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BuildObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("pathObstacle");
        foreach(GameObject obstacle in obstacles )
        {
            Instantiate(obstacleObject, obstacle.transform.position, Quaternion.identity);
            Destroy(obstacle);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
