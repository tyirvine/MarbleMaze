using System.Collections.Generic;

using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{

    // Actuation references
    [Header("Actuation References")]
    public AudioMixer audioMixer;
    public UnityEngine.InputSystem.PlayerInput playerInput;

    // UI References
    [Serializable]
    public class Setting
    {
        public int value = 0;
        public string[] options;
        public TextMeshProUGUI textMeshPro;
        public string rootText;
        public bool justShowValue = false;

        // Constructor
        public Setting(int value, string[] options, string rootText)
        {
            this.value = value;
            this.options = options;
            this.rootText = rootText;
        }
    }

    /* --------------------------- Player preferences --------------------------- */
    [Header("Buttons")]
    public Setting controls = new Setting(0, new string[] { "Keyboard", "Gamepad" }, "Controls:");
    public Setting graphics = new Setting(1, new string[] { "Low", "Medium", "High", "Ultra" }, "Graphics:");
    public Setting resolution = new Setting(1, new string[] { "1280x720", "1920x1080", "2560x1440", "3840x2160" }, "Resolution:");
    public Setting fullscreen = new Setting(0, new string[] { "off", "on" }, "Fullscreen:");

    [Serializable]
    public class SliderSetting
    {
        public float value = 0.5f;
        public Slider slider;
        public TextMeshProUGUI textMeshPro;
        public string rootText;
    }

    [Header("Sliders")]
    public SliderSetting sound;
    public SliderSetting music;

    /// <summary>Resets all score history.</summary>
    public void ResetScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
    }

    /* ----------------------------- Incrementation ----------------------------- */
    /// <summary>The root incrementing method.</summary>
    void Increment(Setting setting)
    {
        // Cap max setting value
        if (setting.value < setting.options.Length - 1 && setting.value >= 0) setting.value++;
        else setting.value = 0;

        UpdateIncrementUI(setting);
    }

    /// <summary>Strictly for updating the ui and nothing else.</summary>
    void UpdateIncrementUI(Setting setting)
    {
        if (!setting.justShowValue)
            setting.textMeshPro.text = setting.rootText + " " + setting.options[setting.value];
        else
            setting.textMeshPro.text = setting.options[setting.value];

        SetControls();
    }

    /// <summary>This updates the actual slider value.</summary>
    void Slider(SliderSetting slider)
    {
        slider.value = slider.slider.value;
        UpdateSliderUI(slider);
    }

    /// <summary>Update the slider's ui by outputting a percentage.</summary>
    public void UpdateSliderUI(SliderSetting slider)
    {
        slider.slider.value = slider.value;
        slider.textMeshPro.text = slider.rootText + " " + (int)(slider.value * 100f) + "%";
        SetControls();
    }

    // These functions are neccessary for ui onclick events to work
    public void IncrementControls() => Increment(controls);
    public void IncrementGraphics() => Increment(graphics);
    public void IncrementResolution() => Increment(resolution);
    public void IncrementFullscreen() => Increment(fullscreen);

    // Sliders
    public void UpdateSound() => Slider(sound);
    public void UpdateMusic() => Slider(music);

    // Set values on awake
    private void Awake()
    {
        // Cycle through all stored values and update them using player prefs
        controls.value = PlayerPrefs.GetInt(nameof(controls), controls.value);
        graphics.value = PlayerPrefs.GetInt(nameof(graphics), graphics.value);
        resolution.value = PlayerPrefs.GetInt(nameof(resolution), resolution.value);
        fullscreen.value = PlayerPrefs.GetInt(nameof(fullscreen), fullscreen.value);

        // Sound
        sound.value = PlayerPrefs.GetFloat(nameof(sound), sound.value);
        music.value = PlayerPrefs.GetFloat(nameof(music), music.value);

        // Update controls
        UpdateIncrementUI(controls);
        UpdateIncrementUI(graphics);
        UpdateIncrementUI(resolution);
        UpdateIncrementUI(fullscreen);
        UpdateSliderUI(sound);
        UpdateSliderUI(music);
    }

    private void Start()
    {
        SetControls();
    }

    /* ------------------------------- Persistence ------------------------------ */
    /// <summary>Save all values. Happens when the player selects done on settings.</summary>
    public void SavePlayerPreferences()
    {
        // Integers
        PlayerPrefs.SetInt(nameof(controls), controls.value);
        PlayerPrefs.SetInt(nameof(graphics), graphics.value);
        PlayerPrefs.SetInt(nameof(resolution), resolution.value);
        PlayerPrefs.SetInt(nameof(fullscreen), fullscreen.value);

        // Floats
        PlayerPrefs.SetFloat(nameof(sound), sound.value);
        PlayerPrefs.SetFloat(nameof(music), music.value);

        // Save to disk
        PlayerPrefs.Save();

        // Apply graphics choice
        QualitySettings.SetQualityLevel(graphics.value);

        // Screen - Find resolution
        Dictionary<int, int[]> resolutionDictionary = new Dictionary<int, int[]>();
        resolutionDictionary.Add(0, new int[] { 1280, 720 });
        resolutionDictionary.Add(1, new int[] { 1920, 1080 });
        resolutionDictionary.Add(2, new int[] { 2560, 1440 });
        resolutionDictionary.Add(3, new int[] { 3840, 2160 });

        // Find full screen
        FullScreenMode fullScreenMode = FullScreenMode.Windowed;
        if (fullscreen.value == 0)
            fullScreenMode = FullScreenMode.Windowed;
        else
            fullScreenMode = FullScreenMode.ExclusiveFullScreen;

        Screen.SetResolution(resolutionDictionary[resolution.value][0], resolutionDictionary[resolution.value][1], fullScreenMode);
    }

    /* -------------------------------- Actuation ------------------------------- */
    /// <summary>This function matches the ui controls to the actual controls in the game.</summary>
    public void SetControls()
    {
        // Sound - Calculate sound levels
        float musicVolume = (1 - music.value) * -80f;
        float soundVolume = (1 - sound.value) * -80f;

        audioMixer.SetFloat("musicVolume", musicVolume);
        audioMixer.SetFloat("sfxVolume", soundVolume);

        // Controls
        playerInput.defaultControlScheme = controls.options[controls.value];
    }

}
