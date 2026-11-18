using UnityEngine;

/// <summary>
/// Стрелка на жёлтом блоке. Задаёт направление движения.
/// Вращается по взаимодействию (E) независимо от цвета блока, места прицеливания
/// и того, движется ли блок. Двигаться блок начинает только когда он жёлтый
/// (это решает YellowMovingBlock).
/// </summary>
public class DirectionArrow : MonoBehaviour
{
    [Header("Состояние")]
    [Tooltip("Активна ли стрелка. Если выключена — блок ведёт себя как обычный (гравитация и толкание), стрелка не вращается.")]
    public bool isActive = true;

    [Header("Взаимодействие")]
    [Tooltip("Выключено: стрелка вращается при взаимодействии с любой частью блока. " +
             "Включено: только если игрок целится рядом со стрелкой (Arrow Hit Radius).")]
    [SerializeField] private bool requireAimNearArrow = false;
    [SerializeField] private float arrowHitRadius = 0.5f;

    [Header("Поворот")]
    [SerializeField] private float rotationStep = 90f;

    [Header("Визуал")]
    [Tooltip("Модель стрелки. Если не задана, при выключении стрелки отключается весь её объект.")]
    [SerializeField] private GameObject visualObject;

    private void Start()
    {
        UpdateVisibility();
    }

    /// <summary>Включить или выключить стрелку (например, из переключателя).</summary>
    public void SetActive(bool state)
    {
        isActive = state;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (visualObject != null)
            visualObject.SetActive(isActive);
        else
            gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Повернуть стрелку. Возвращает true, если поворот произошёл.
    /// Не зависит от цвета блока и от того, движется ли он.
    /// </summary>
    public bool TryRotate(Vector3 hitPoint)
    {
        if (!isActive) return false;

        if (requireAimNearArrow &&
            Vector3.Distance(hitPoint, transform.position) > arrowHitRadius)
            return false;

        transform.Rotate(0f, rotationStep, 0f, Space.Self);
        return true;
    }

    public Vector3 ForwardDirection => transform.forward;
}