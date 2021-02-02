using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // Settings
    public float transitionInSeconds = 1.0f;
    public float hueStep = 1.0f;
    // https://forum.unity.com/threads/equivalent-to-lerp-or-smoothstep-but-with-custom-curves.229966/
    public AnimationCurve lerpCurve;

    // State objects
    public bool changeColor = false;
    float time = 0.0f;

    // Materials
    /// <summary>The actual background colour.</summary>
    public Material mat_background;
    /// <summary>The floor of a board.</summary>
    public Material mat_foreground;
    /// <summary>The walls of a board.</summary>
    public Material mat_accent;

    public class HueObject
    {
        public Material material;
        public Color startColor;
        public Color endColor;

        public HueObject(Material material)
        {
            this.material = material;
        }
    }

    // Hues
    public HueObject hue_background;
    public HueObject hue_foreground;
    public HueObject hue_accent;

    /// <summary>This changes the hue for one material. It's a heavily used method.</summary>
    public Color FindNewHue(Material material)
    {
        // Calculate HSV from colour
        float H, S, V;
        Color.RGBToHSV(material.color, out H, out S, out V);

        // Adjust hue
        float possibleHue = H + hueStep;
        if (possibleHue > 359f) H = 0f;
        else H = possibleHue;

        // Convert back to color
        return Color.HSVToRGB(H, S, V);
    }

    /// <summary>This method simply engages the colour change of all colours.</summary>
    public void ChangeMaterial(HueObject hueObject)
    {
        if (time == 0f)
        {
            hueObject.startColor = hueObject.material.color;
            hueObject.endColor = FindNewHue(hueObject.material);
        }

        hueObject.material.color = Color.Lerp(hueObject.startColor, hueObject.endColor, lerpCurve.Evaluate(time));
    }

    // Assign materials initially
    void Awake()
    {
        // hue_background.material = mat_background;
        hue_foreground = new HueObject(mat_foreground);
        hue_accent = new HueObject(mat_accent);
    }

    // Update is called once per frame
    void Update()
    {
        if (changeColor)
        {
            if (time < transitionInSeconds)
            {
                // ChangeMaterial(hue_background);
                ChangeMaterial(hue_foreground);
                ChangeMaterial(hue_accent);
                time += Time.deltaTime;
            }
            else
            {
                changeColor = false;
                time = 0.0f;
            }
        }
    }
}
