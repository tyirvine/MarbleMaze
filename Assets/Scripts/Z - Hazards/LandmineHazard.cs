using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineHazard : MonoBehaviour
{

    // Settings
    public AnimationCurve fadeCurve;
    [Range(0f, 10f)] public float fadeRate;

    // Min / max values
    [Header("Min/Max Values")]
    public float lightMin = 0.0f;
    public float lightMax = 1.0f;
    public Color materialMin;
    public Color materialMax;

    // References
    [Header("References")]
    public Light buttonLight;
    public BoxCollider buttonCollider;
    public Renderer buttonRenderer;

    // State objects
    float fadewave_light = 0.0f;
    Color fadewave_color;
    float time = 0.0f;

    /// <summary>This function provides a lerp based on a provided min/max.</summary>
    float FadeLerp(float min, float max) => Mathf.Lerp(min, max, fadeCurve.Evaluate(time));

    // Fade the light in and out
    private void Update()
    {
        // Calculate current wave value based on time
        fadewave_light = FadeLerp(lightMin, lightMax);
        fadewave_color = Color.Lerp(materialMin, materialMax, fadeCurve.Evaluate(time));

        // Plug lerp calc.s into objects
        buttonLight.range = fadewave_light;
        buttonRenderer.material.SetColor("_EmissionColor", fadewave_color);

        // Calculate current time
        time += fadeRate * Time.deltaTime;

        // Reset time once it surpasses 1 second
        if (time >= 1.0)
        {
            time = 0.0f;
        }
    }
    // Fade the material emission in and out

    // When the landmine is triggered set the land mine to a state of inactivity

}
