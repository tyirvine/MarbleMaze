using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	//This will start the game on the first Index in the Build Setting, currently it only has 1 level for prototyping purposes
	public void PlayGame() {

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

	}

	//This Function Closes the Application
	public void QuitGame() {
		Debug.Log("Quit the Game");
		Application.Quit();
	}


}