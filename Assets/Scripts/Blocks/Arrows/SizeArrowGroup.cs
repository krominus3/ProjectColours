using System;
using UnityEngine;

/// <summary>
/// Общее состояние угловых стрелок блока: наружу / внутрь.
/// Вешается на объект SizeArrows (родитель четырёх SizeArrow).
/// Нажатие на любую стрелку переключает состояние всех стрелок сразу.
/// Смысл состояния задаёт цвет блока:
///   синий    — наружу = рост (Grow),        внутрь = уменьшение (Shrink)
///   бирюзовый — наружу = отталкивание,      внутрь = притяжение
/// Состояние общее, поэтому блок, меняющий цвет, сохраняет выбранный значок.
/// </summary>
public class SizeArrowGroup : MonoBehaviour
{
    [Tooltip("Начальное состояние: true = стрелки наружу, false = внутрь.")]
    [SerializeField] private bool outward = true;

    [Tooltip("Если пусто, стрелки ищутся среди дочерних объектов.")]
    [SerializeField] private SizeArrow[] arrows;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Material[] inOut;

    public bool IsOutward => outward;

    /// <summary>Состояние изменилось (параметр: true = наружу).</summary>
    public event Action<bool> Changed;

    private void Awake()
    {
        if (arrows == null || arrows.Length == 0)
            arrows = GetComponentsInChildren<SizeArrow>(true);
    }

    private void OnEnable()
    {
        foreach (var arrow in arrows)
            if (arrow != null) arrow.Interacted += OnArrowInteracted;
    }

    private void OnDisable()
    {
        foreach (var arrow in arrows)
            if (arrow != null) arrow.Interacted -= OnArrowInteracted;
    }

    private void Start()
    {
        ApplyVisuals();
    }

    // Нажатие на любую стрелку -> переключаем один раз на нажатие
    private void OnArrowInteracted(BlockArrow _)
    {
        Toggle();
    }

    public void Toggle() => Set(!outward);

    public void Set(bool value)
    {
        if (outward == value) return;
        outward = value;
        ApplyVisuals();
        Changed?.Invoke(outward);
    }

    private void ApplyVisuals()
    {
        mesh.material = outward ? inOut[1] : inOut[0];
    }
}