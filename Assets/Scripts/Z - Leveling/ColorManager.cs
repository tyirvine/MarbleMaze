using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // Settings
    public float transitionInSeconds = 1.0f;
    public float hueStep = 0.1f;
    // https://forum.unity.com/threads/equivalent-to-lerp-or-smoothstep-but-with-custom-curves.229966/
    public AnimationCurve lerpCurve;

    // State objects
    [HideInInspector] public ChangeColor changeColor = ChangeColor.Disabled;
    float time = 0.0f;

    /// <summary>Simplifies changeColor states.</summary>
    public enum ChangeColor
    {
        Disabled,
        Enabled,
        Revert
    }

    // Materials
    /// <summary>The floor of a board.</summary>
    public Material mat_foreground;
    /// <summary>The walls of a board.</summary>
    public Material mat_accent;
    /// <summary>The material for the UI level counter.</summary>
    public Material mat_userInterface;

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

    /// <summary>Changes the color for the user interface counter.</summary>
    void UpdateUIColor() => mat_userInterface.SetColor("_UnderlayColor", hue_foreground.material.color);

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

    /// <summary>This shifts the colours back to the starting hue.</summary>
    public void RevertMaterial(HueObject hueObject)
    {
        if (time == 0f)
        {
            if (hueObject.material != null)
            {
                hueObject.startColor = hueObject.material.color;
                hueObject.endColor = ShiftHue(hueObject.material.color, startingHue);
            }
            else if (hueObject.camera != null)
            {
                hueObject.startColor = hueObject.camera.backgroundColor;
                hueObject.endColor = ShiftHue(hueObject.camera.backgroundColor, startingHue);
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

        // Establish ui colour
        UpdateUIColor();
    }

    // Update is called once per frame
    void Update()
    {
        switch (changeColor)
        {
            // Normal color shift
            case ChangeColor.Enabled:
                if (time < transitionInSeconds)
                {
                    ChangeMaterial(hue_background);
                    ChangeMaterial(hue_foreground);
                    ChangeMaterial(hue_accent);
                    UpdateUIColor();
                    time += Time.deltaTime;
                }
                else
                {
                    changeColor = ChangeColor.Disabled;
                    time = 0f;
                }
                break;

            // Revert colors to starting hue
            case ChangeColor.Revert:
                if (time < transitionInSeconds)
                {
                    RevertMaterial(hue_background);
                    RevertMaterial(hue_foreground);
                    RevertMaterial(hue_accent);
                    time += Time.deltaTime;
                }
                else
                {
                    changeColor = ChangeColor.Disabled;
                    time = 0f;
                }
                break;

            default:
                break;
        }
    }
}
