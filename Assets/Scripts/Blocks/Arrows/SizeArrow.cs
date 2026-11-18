using UnityEngine;

/// <summary>
/// Угловая стрелка синего блока: показывает режим размера.
/// Наружу = Grow (блок растёт), внутрь = Shrink (блок уменьшается).
/// Режим хранит и переключает BlueSizeBlock (подписан на событие Interacted),
/// стрелка только отображает его. Нужно целиться именно в стрелку (коллайдер на ней).
/// Ставится в угол одной грани блока; по умолчанию смотрит НАРУЖУ (от центра блока).
/// </summary>
public class SizeArrow : BlockArrow
{
    [Header("Вид режимов")]
    [Tooltip("Необязательно: отдельная модель для режима «наружу».")]
    [SerializeField] private GameObject outwardVisual;
    [Tooltip("Необязательно: отдельная модель для режима «внутрь».")]
    [SerializeField] private GameObject inwardVisual;
    [Tooltip("Если отдельные модели не заданы, стрелка разворачивается на 180° вокруг этой локальной оси " +
             "(для стрелки, лежащей на грани, это обычно нормаль грани).")]
    [SerializeField] private Vector3 flipAxis = Vector3.up;

    private Quaternion _outwardRotation;
    private bool _rotationCaptured;

    private void Awake()
    {
        CaptureRotation();
    }

    private void CaptureRotation()
    {
        if (_rotationCaptured) return;
        _outwardRotation = transform.localRotation;
        _rotationCaptured = true;
    }

    /// <summary>Показать режим: Grow = наружу, Shrink = внутрь.</summary>
    public void ShowMode(BlueSizeBlock.SizeMode mode)
    {
        CaptureRotation();
        bool outward = mode == BlueSizeBlock.SizeMode.Grow;

        if (outwardVisual != null || inwardVisual != null)
        {
            if (outwardVisual != null) outwardVisual.SetActive(outward);
            if (inwardVisual != null) inwardVisual.SetActive(!outward);
        }
        else
        {
            transform.localRotation = outward
                ? _outwardRotation
                : _outwardRotation * Quaternion.AngleAxis(180f, flipAxis);
        }
    }
}