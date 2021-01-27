using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public int score = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("pickup"))
        {
            score += other.GetComponent<Pill>().pickup.pickupValue;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("LevelFinish"))
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
            gm.GetComponent<GameManager>().ReloadLevel();
        }
        Debug.Log(other.tag);
    }
}
