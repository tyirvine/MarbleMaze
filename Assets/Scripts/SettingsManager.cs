
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
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
        // Reset score here
    }

    /* ----------------------------- Incrementation ----------------------------- */
    /// <summary>The root incrementing method.</summary>
    void Increment(Setting setting)
    {
        if (setting.value < setting.options.Length - 1 && setting.value >= 0) setting.value++;
        else setting.value = 0;

        // Update text
        if (!setting.justShowValue)
            setting.textMeshPro.text = setting.rootText + " " + setting.options[setting.value];
        else
            setting.textMeshPro.text = setting.options[setting.value];
    }

    /// <summary>Update the slider's ui by outputting a percentage.</summary>
    public void UpdateSliderUI(SliderSetting slider) => slider.textMeshPro.text = slider.rootText + " " + (int)(slider.slider.value * 100f) + "%";

    // These functions are neccessary for ui onclick events to work
    public void IncrementControls() => Increment(controls);
    public void IncrementGraphics() => Increment(graphics);
    public void IncrementResolution() => Increment(resolution);
    public void IncrementFullscreen() => Increment(fullscreen);

    // Sliders
    public void UpdateSound() => UpdateSliderUI(sound);
    public void UpdateMusic() => UpdateSliderUI(music);

    // Set values on awake
    private void Awake()
    {
        // Cycle through all stored values and update them using player prefs
        controls.value = PlayerPrefs.GetInt(nameof(controls), controls.value);
        graphics.value = PlayerPrefs.GetInt(nameof(graphics), graphics.value);
        resolution.value = PlayerPrefs.GetInt(nameof(resolution), resolution.value);
        fullscreen.value = PlayerPrefs.GetInt(nameof(fullscreen), fullscreen.value);

        // Update controls
        IncrementControls();
        IncrementGraphics();
        IncrementResolution();
        IncrementFullscreen();
        UpdateSound();
        UpdateMusic();
    }

    /* ------------------------------- Persistence ------------------------------ */
    /// <summary>Save all values.</summary>
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
    }
}
