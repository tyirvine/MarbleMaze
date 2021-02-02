using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // Settings
    public float transitionInSeconds = 1.0f;
    public float hueStep = 0.1f;
    // https://forum.unity.com/threads/equivalent-to-lerp-or-smoothstep-but-with-custom-curves.229966/
    public AnimationCurve lerpCurve;

    // State objects
    [HideInInspector] public bool changeColor = false;
    float time = 0.0f;

    // Materials
    /// <summary>The floor of a board.</summary>
    public Material mat_foreground;
    /// <summary>The walls of a board.</summary>
    public Material mat_accent;

    // Colors
    public Color startingHue;
    public Color color_foreground;
    public Color color_accent;

    /// <summary>This object can change the color of the material it's attached to.</summary>
    public class HueObject
    {
        public Camera camera;
        public Material material;
        public Color startColor;
        public Color endColor;

        public HueObject(Material material)
        {
            this.material = material;
        }
        public HueObject(Camera camera)
        {
            this.camera = camera;
        }
    }

    // Hues
    public HueObject hue_background;
    public HueObject hue_foreground;
    public HueObject hue_accent;

    /// <summary>This shifts the hue to the starting colour.</summary>
    public Color ShiftHue(Color startColor, Color targetHue)
    {
        // Calculate HSV from colour
        float startH, startS, startV = 0f;
        Color.RGBToHSV(startColor, out startH, out startS, out startV);

        // Calculate HSV from target
        float targetH, targetS, targetV = 0f;
        Color.RGBToHSV(targetHue, out targetH, out targetS, out targetV);

        // Swap hue
        startH = targetH;

        return Color.HSVToRGB(startH, startS, startV);
    }

    /// <summary>This changes the hue for one material. It's a heavily used method.</summary>
    public Color FindNewHue(Color color)
    {
        // Calculate HSV from colour
        float H, S, V = 0f;
        Color.RGBToHSV(color, out H, out S, out V);

        Debug.Log(H);

        // Adjust hue
        H += hueStep;

        // Convert back to color
        return Color.HSVToRGB(H, S, V);
    }

    /// <summary>This method simply engages the colour change of all colours.</summary>
    public void ChangeMaterial(HueObject hueObject)
    {
        if (time == 0f)
        {
            if (hueObject.material != null)
            {
                hueObject.startColor = hueObject.material.color;
                hueObject.endColor = FindNewHue(hueObject.material.color);
            }
            else if (hueObject.camera != null)
            {
                hueObject.startColor = hueObject.camera.backgroundColor;
                hueObject.endColor = FindNewHue(hueObject.camera.backgroundColor);
            }
        }

        if (hueObject.material != null)
            hueObject.material.color = Color.Lerp(hueObject.startColor, hueObject.endColor, lerpCurve.Evaluate(time));
        else if (hueObject.camera != null)
            hueObject.camera.backgroundColor = Color.Lerp(hueObject.startColor, hueObject.endColor, lerpCurve.Evaluate(time));
    }

    // Assign materials initially
    void Awake()
    {
        Camera camera = FindObjectOfType<Camera>();
        // Set initial colours
        mat_foreground.color = color_foreground;
        mat_accent.color = color_accent;

        // Shift initial colours to designated hue
        camera.backgroundColor = ShiftHue(camera.backgroundColor, startingHue);
        mat_foreground.color = ShiftHue(mat_foreground.color, startingHue);
        mat_accent.color = ShiftHue(mat_accent.color, startingHue);

        // Create hue objects
        hue_background = new HueObject(camera);
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
                ChangeMaterial(hue_background);
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
