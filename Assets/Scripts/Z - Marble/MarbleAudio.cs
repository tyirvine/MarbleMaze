using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleAudio : MonoBehaviour {
	// Start is called before the first frame update
	Rigidbody marbleRigidbody;
	AudioSource marbleAudio;

	void Start() {
		marbleRigidbody = gameObject.GetComponent<Rigidbody>();
		marbleAudio = gameObject.GetComponent<AudioSource>();

	}

	// Update is called once per frame
	void Update() {
		if (marbleRigidbody.velocity.magnitude > 0) {
			Debug.Log("Moving");
			if (!marbleAudio.isPlaying) {
				marbleAudio.Play();

			}
			marbleAudio.volume = marbleRigidbody.velocity.magnitude / 20f;
		} else {
			marbleAudio.Pause();
		}
	}
}
