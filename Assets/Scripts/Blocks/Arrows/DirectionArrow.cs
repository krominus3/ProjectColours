using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Стрелка направления на жёлтом блоке.
/// Вращается по взаимодействию (E) независимо от цвета блока и того, движется ли он.
/// Движением занимается YellowMovingBlock (подписан на событие Interacted).
/// </summary>
public class DirectionArrow : BlockArrow
{
    [Header("Взаимодействие")]
    [Tooltip("Выключено: стрелка реагирует на любую часть блока. " +
             "Включено: только если игрок целится в саму стрелку (нужен коллайдер на стрелке).")]
    [FormerlySerializedAs("requireAimNearArrow")]
    [SerializeField] private bool requireAimAtArrow = false;

    [Header("Поворот")]
    [SerializeField] private float rotationStep = 90f;

    public override bool AcceptsAnyPartOfBlock => !requireAimAtArrow;

    protected override void OnInteract()
    {
        transform.Rotate(0f, rotationStep, 0f, Space.Self);
    }

    public Vector3 ForwardDirection => transform.forward;
}