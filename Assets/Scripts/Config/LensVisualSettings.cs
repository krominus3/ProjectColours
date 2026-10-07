using UnityEngine;

[CreateAssetMenu(menuName = "Lens/Visual Settings")]
public class LensVisualSettings : ScriptableObject
{
    public LensType type;

    [Header("Color Adjustments")]
    public float saturation = 0f;
    public float contrast = 0f;
    public float postExposure = 0f;
    public float hueShift = 0f;
    public Color colorFilter = Color.white;

    [Header("Channel Mixer")]
    [Range(-200, 200)] public float redR = 100f;
    [Range(-200, 200)] public float greenG = 100f;
    [Range(-200, 200)] public float blueB = 100f;
    [Range(-200, 200)] public float redG = 0f;
    [Range(-200, 200)] public float redB = 0f;
    [Range(-200, 200)] public float greenR = 0f;
    [Range(-200, 200)] public float greenB = 0f;
    [Range(-200, 200)] public float blueR = 0f;
    [Range(-200, 200)] public float blueG = 0f;

    [Header("Vignette")]
    public float vignetteIntensity = 0f;
    public Color vignetteColor = Color.black;

    [Header("Bloom")]
    public float bloomIntensity = 0f;

    [Header("Lens Distortion")]
    public float distortion = 0f;

    [Header("Transition")]
    public float transitionDuration = 0.35f;
}