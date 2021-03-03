using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float forceX = 1f;
    public float forceZ = 1f;

    public GameObject marblePrefab;
    public GameObject dropPosition;

    // Drops the marble into the scene
    public void DropMarble()
    {
        GameObject marble = Instantiate(marblePrefab, dropPosition.transform.position, Quaternion.identity);
        marble.GetComponent<Rigidbody>().AddForce(new Vector3(1f * forceX, 0f, 1f * forceZ), ForceMode.Force);
    }

    // Loads game view scene in
    public void LoadNextScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex + 1);
    }
}
