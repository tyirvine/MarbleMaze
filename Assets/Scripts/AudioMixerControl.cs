using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerControl : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetSFXLevel(float level)
    {
        audioMixer.SetFloat("sfxVolume", level);
    }

    public void SetMusicLevel(float level)
    {
        audioMixer.SetFloat("musicVolume", level);
    }

    public void SetMasterLevel(float level)
    {
        audioMixer.SetFloat("masterVolume", level);
    }


}
