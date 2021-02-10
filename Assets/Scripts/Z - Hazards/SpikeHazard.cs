using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    // References 
    public StatsManager statsManager;

    // Grab stats manager
    private void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
    }

    // Detect for ball collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //might be better to just grab the marble script and do the changes there `getcomponent<marblebehaviour>` and do like "DeathSequence" ?
            other.GetComponent<MeshRenderer>().enabled = false;
            //playsound, this might be triggering twice?
            other.GetComponent<MarbleBehaviour>().PlayAudio(other.GetComponent<MarbleBehaviour>().deathSound);
            statsManager.RemoveLife();
        }
    }
}
