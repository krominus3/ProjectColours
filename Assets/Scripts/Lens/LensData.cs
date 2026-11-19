using UnityEngine;

[CreateAssetMenu(menuName = "Lens/LensData")]
public class LensData : ScriptableObject
{
    public LensType type;
    public Color tintColor = Color.white;
    public Sprite icon;

    [Header("Визуал мира")]
    public Color worldTint = Color.white;      // цвет затемнения камеры
    public float worldSaturation = 1f;
    public float worldContrast = 1f;
}