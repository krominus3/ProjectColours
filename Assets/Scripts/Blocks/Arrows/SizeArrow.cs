using UnityEngine;

/// <summary>
/// Угловая стрелка блока: показывает режим «наружу / внутрь».
/// Что это значит, решает цвет: синий — рост / уменьшение, бирюзовый — отталкивание / притяжение.
/// Состояние хранит и переключает SizeArrowGroup (подписан на событие Interacted),
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

    /// <summary>Показать режим: true = стрелка наружу, false = внутрь.</summary>
    public void ShowOutward(bool outward)
    {
        CaptureRotation();

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