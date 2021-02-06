using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineHazard : MonoBehaviour
{

    // Settings
    public AnimationCurve fadeCurve;
    [Range(0f, 10f)] public float fadeRate;
    [Range(0f, 10f)] public float detonationFadeRate;

    // Explosion Settings
    [Header("Explosion Settings")]
    public float explosionForce;
    public float explosionRadius;
    [Range(0.0f, 100.0f)] public float upwardsModifier;
    public ForceMode explosionForceMode = ForceMode.Impulse;

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
    public Animator buttonAnimator;
    public GameObject wallEviscerator;

    // Detonation phases
    enum DetonationPhases
    {
        PreDetonated,
        Detonating,
        PostDetonation
    }

    // State objects
    float fadewave_light = 0.0f;
    Color fadewave_color;
    float time = 0.0f;
    bool landmineTrigger = false;
    DetonationPhases landmineState = DetonationPhases.PreDetonated;

    /// <summary>This function provides a lerp based on a provided min/max.</summary>
    float FadeLerp(float min, float max) => Mathf.Lerp(min, max, fadeCurve.Evaluate(time));

    /// <summary>Creates a sphere around the land mine that picksup collider references.
    /// Then it goes through each reference and applies an explosion force to it's rigidbody.</summary>
    void LandmineExplode()
    {
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitObject in hitObjects)
        {
            // Check to make sure there's a rigidbody first
            if (hitObject.tag == "wallTile" && hitObject.gameObject.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rigidbody = hitObject.gameObject.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
                rigidbody.AddExplosionForce(explosionForce, gameObject.transform.position, explosionRadius, upwardsModifier, explosionForceMode);
            }
        }
    }

    // This is a detonation trigger for the landmine
    public void DetonateLandmine()
    {
        LandmineExplode();
        landmineState = DetonationPhases.Detonating;
    }

    // Fade the light in and out
    private void Update()
    {
        if (landmineState == DetonationPhases.PreDetonated)
        {

            // Calculate current wave value based on time
            fadewave_light = FadeLerp(lightMin, lightMax);
            fadewave_color = Color.Lerp(materialMin, materialMax, fadeCurve.Evaluate(time));

            // Plug lerp calc.s into objects
            buttonLight.range = fadewave_light;
            buttonRenderer.material.SetColor("_EmissionColor", fadewave_color);

            // This is the detonation process for the landmine
            if (landmineTrigger)
            {
                fadeRate = detonationFadeRate;
                Invoke("DetonateLandmine", 1.3f);
            }

            // Calculate current time
            time += fadeRate * Time.deltaTime;

            // Reset time once it surpasses 1 second
            if (time >= 1.0)
            {
                time = 0.0f;
            }
        }
        else if (landmineState == DetonationPhases.Detonating)
        {
            // After detonation set the landmine to a rest state
            buttonLight.range = lightMin;
            buttonRenderer.material.SetColor("_EmissionColor", materialMin);
            landmineState = DetonationPhases.PostDetonation;
        }
    }

    // When the landmine is triggered set the land mine to a state of inactivity

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            landmineTrigger = true;
            buttonAnimator.SetBool("Pressed", landmineTrigger);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        buttonAnimator.SetBool("Pressed", false);
    }
}
