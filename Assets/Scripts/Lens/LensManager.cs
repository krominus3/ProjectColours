using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LensManager : MonoBehaviour
{
    [SerializeField] private LensInventory inventory;
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private LensVisualSettings[] visualSettings;
    [SerializeField] private LensVisualSettings defaultSettings; // для LensType.None

    [Header("Transition")]
    [SerializeField] private bool smoothTransition = true;

    private ColorAdjustments _colorAdj;
    private ChannelMixer _channelMixer;
    private Vignette _vignette;
    private Bloom _bloom;
    private LensDistortion _distortion;

    // Целевые значения
    private LensVisualSettings _target;

    // Текущие интерполируемые значения
    private float _t; // 0..1 прогресс перехода
    private LensVisualSettings _from;
    private float _transitionDuration = 0.35f;

    private void Awake()
    {
        if (inventory == null)
        {
            Debug.LogError("Не подключен инвентарь!");
            inventory = FindAnyObjectByType<LensInventory>();
        }

        if (postProcessVolume == null || postProcessVolume.profile == null)
        {
            Debug.LogError("Не подключен Volume с профилем!");
            return;
        }

        // Достаём оверрайды из профиля
        postProcessVolume.profile.TryGet(out _colorAdj);
        postProcessVolume.profile.TryGet(out _channelMixer);
        postProcessVolume.profile.TryGet(out _vignette);
        postProcessVolume.profile.TryGet(out _bloom);
        postProcessVolume.profile.TryGet(out _distortion);
    }

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnLensChanged += OnLensChanged;
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnLensChanged -= OnLensChanged;
    }

    private void OnLensChanged(LensType type)
    {
        var settings = FindSettings(type);
        _from = _target ?? defaultSettings; // текущий как «от»
        _target = settings;
        _t = 0f;
        _transitionDuration = settings != null ? settings.transitionDuration : 0.35f;

        // Сообщаем блокам сразу (реакция не должна ждать плавности)
        foreach (var block in FindObjectsByType<ColorBlock>(FindObjectsSortMode.None))
            block.ReactToLens(type);
    }

    private void Update()
    {
        if (_target == null) return;

        if (smoothTransition && _t < 1f)
        {
            _t += Time.deltaTime / Mathf.Max(0.01f, _transitionDuration);
            _t = Mathf.Clamp01(_t);
        }
        else
        {
            _t = 1f;
        }

        ApplyBlend(_from, _target, _t);
    }

    private void ApplyBlend(LensVisualSettings a, LensVisualSettings b, float t)
    {
        if (b == null) return;
        if (a == null) a = b;

        // Color Adjustments
        if (_colorAdj != null)
        {
            _colorAdj.saturation.value = Mathf.Lerp(a.saturation, b.saturation, t);
            _colorAdj.contrast.value = Mathf.Lerp(a.contrast, b.contrast, t);
            _colorAdj.postExposure.value = Mathf.Lerp(a.postExposure, b.postExposure, t);
            _colorAdj.hueShift.value = Mathf.Lerp(a.hueShift, b.hueShift, t);
            _colorAdj.colorFilter.value = Color.Lerp(a.colorFilter, b.colorFilter, t);
        }

        // Channel Mixer
        if (_channelMixer != null)
        {
            _channelMixer.redOutRedIn.value = Mathf.Lerp(a.redR, b.redR, t);
            _channelMixer.greenOutGreenIn.value = Mathf.Lerp(a.greenG, b.greenG, t);
            _channelMixer.blueOutBlueIn.value = Mathf.Lerp(a.blueB, b.blueB, t);
            _channelMixer.redOutGreenIn.value = Mathf.Lerp(a.redG, b.redG, t);
            _channelMixer.redOutBlueIn.value = Mathf.Lerp(a.redB, b.redB, t);
            _channelMixer.greenOutRedIn.value = Mathf.Lerp(a.greenR, b.greenR, t);
            _channelMixer.greenOutBlueIn.value = Mathf.Lerp(a.greenB, b.greenB, t);
            _channelMixer.blueOutRedIn.value = Mathf.Lerp(a.blueR, b.blueR, t);
            _channelMixer.blueOutGreenIn.value = Mathf.Lerp(a.blueG, b.blueG, t);
        }

        // Vignette
        if (_vignette != null)
        {
            _vignette.intensity.value = Mathf.Lerp(a.vignetteIntensity, b.vignetteIntensity, t);
            _vignette.color.value = Color.Lerp(a.vignetteColor, b.vignetteColor, t);
        }

        // Bloom
        if (_bloom != null)
        {
            _bloom.intensity.value = Mathf.Lerp(a.bloomIntensity, b.bloomIntensity, t);
        }

        // Lens Distortion
        if (_distortion != null)
        {
            _distortion.intensity.value = Mathf.Lerp(a.distortion, b.distortion, t);
        }
    }

    private LensVisualSettings FindSettings(LensType type)
    {
        foreach (var s in visualSettings)
            if (s != null && s.type == type) return s;
        return defaultSettings;
    }
}