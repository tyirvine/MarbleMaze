using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    // Grab player's position
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<GameManager>().marble;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.transform.position;
    }
}
