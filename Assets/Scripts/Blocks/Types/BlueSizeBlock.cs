using UnityEngine;

/// <summary>
/// Синий цвет: блок меняет размер.
/// Grow   — пропорциональный рост в сторону свободного места, пока не упрётся в стенки.
/// Shrink — уменьшение до 1/4 в сторону прилегающей поверхности
///          (если поверхностей несколько или их нет — в центр).
/// Блоки считаются осевыми (без поворота), размер = localScale (куб 1x1x1).
/// </summary>
public class BlueSizeBlock : MonoBehaviour
{
    public enum SizeMode { Grow, Shrink }

    [Header("Режим работы")]
    [Tooltip("Используется, только если на блоке нет группы угловых стрелок (SizeArrowGroup). " +
             "Grow = растёт в сторону свободного места. Shrink = уменьшается до 1/4 к стенке или в центр.")]
    [SerializeField] private SizeMode mode = SizeMode.Grow;

    [Header("Стрелки")]
    [Tooltip("Группа угловых стрелок (SizeArrowGroup). Если пусто, ищется среди дочерних объектов. " +
             "Стрелки наружу = Grow, внутрь = Shrink.")]
    [SerializeField] private SizeArrowGroup arrowGroup;

    [Header("Рост")]
    [Tooltip("Максимальный множитель размера (рост пропорциональный по всем осям).")]
    [SerializeField] private float maxGrowMultiplier = 2f;

    [Header("Уменьшение")]
    [SerializeField] private float minScaleMultiplier = 0.25f;

    [Header("Анимация")]
    [Tooltip("Чем больше, тем быстрее (3 = около 0.33 сек).")]
    [SerializeField] private float changeSpeed = 3f;

    [Header("Физика")]
    [SerializeField] private LayerMask wallMask = ~0;
    [Tooltip("Небольшой зазор, чтобы блок не считал стенку, к которой прижат, пересечением.")]
    [SerializeField] private float skin = 0.02f;
    [Tooltip("Насколько близко стенка считается прилегающей (для уменьшения).")]
    [SerializeField] private float touchTolerance = 0.05f;

    private ColoredBlock _block;
    private Transform _root;

    // Исходное состояние блока (до синего эффекта)
    private Vector3 _baseScale;
    private Vector3 _basePosition;
    private bool _modified;          // true, пока блок отличается от исходного (эффект идёт или ещё возвращается)

    // Анимация
    private Vector3 _startScale, _startPos;
    private Vector3 _targetScale, _targetPos;
    private float _progress = 1f;

    private bool _wasBlue;

    private readonly Collider[] _overlapBuf = new Collider[32];
    private readonly RaycastHit[] _hitBuf = new RaycastHit[32];

    // Текущий режим: если на блоке есть группа угловых стрелок — берём из неё
    // (наружу = Grow, внутрь = Shrink), иначе из поля mode.
    private SizeMode CurrentMode =>
        arrowGroup != null ? (arrowGroup.IsOutward ? SizeMode.Grow : SizeMode.Shrink) : mode;

    private void Awake()
    {
        _block = GetComponentInParent<ColoredBlock>();
        _root = _block != null ? _block.transform : transform;

        if (arrowGroup == null)
            arrowGroup = _root.GetComponentInChildren<SizeArrowGroup>(true);

        SaveBaseTransform();
    }

    private void OnEnable()
    {
        if (arrowGroup != null) arrowGroup.Changed += OnArrowModeChanged;
    }

    private void OnDisable()
    {
        if (arrowGroup != null) arrowGroup.Changed -= OnArrowModeChanged;
    }

    // Игрок переключил стрелки. Работает при любом цвете блока;
    // если блок сейчас синий — плавно переходим к новому размеру.
    private void OnArrowModeChanged(bool outward)
    {
        Recalculate();
    }

    /// <summary>
    /// Запомнить текущие размер и позицию как исходные.
    /// Вызывается из YellowMovingBlock после остановки. Пока синий эффект активен, игнорируется.
    /// </summary>
    public void SaveBaseTransform()
    {
        if (_modified) return;
        _baseScale = _root.localScale;
        _basePosition = _root.position;
    }

    /// <summary>
    /// Пересчитать цель, если окружение изменилось, пока блок синий
    /// (например, исчезла стена). Можно вызвать после смены линзы.
    /// </summary>
    public void Recalculate()
    {
        if (_wasBlue) StartBlueEffect();
    }

    private void Update()
    {
        if (_block == null) return;

        bool isBlue = _block.currentColor == LensType.Blue;
        if (isBlue != _wasBlue)
        {
            _wasBlue = isBlue;
            if (isBlue) StartBlueEffect();
            else StartReturn();
        }

        Animate();
    }

    // ---------- Запуск эффекта ----------

    private void StartBlueEffect()
    {
        // Исходное состояние запоминаем только если блок сейчас "чистый".
        // Если он ещё возвращается обратно — база остаётся прежней.
        if (!_modified)
        {
            SaveBaseTransform();
            _modified = true;
        }

        Vector3 newScale, newPos;
        if (CurrentMode == SizeMode.Grow) CalculateGrow(out newScale, out newPos);
        else CalculateShrink(out newScale, out newPos);

        BeginTransition(newScale, newPos);
    }

    private void StartReturn()
    {
        if (!_modified) return;
        BeginTransition(_baseScale, _basePosition);
    }

    private void BeginTransition(Vector3 scale, Vector3 pos)
    {
        _startScale = _root.localScale;
        _startPos = _root.position;
        _targetScale = scale;
        _targetPos = pos;
        _progress = 0f;
    }

    private void Animate()
    {
        if (_progress >= 1f) return;

        _progress = Mathf.Min(1f, _progress + Time.deltaTime * changeSpeed);
        float t = Mathf.SmoothStep(0f, 1f, _progress);

        _root.localScale = Vector3.Lerp(_startScale, _targetScale, t);
        _root.position = Vector3.Lerp(_startPos, _targetPos, t);

        if (_progress >= 1f && !_wasBlue)
            _modified = false; // вернулись в исходное состояние
    }

    // ---------- Рост ----------

    private void CalculateGrow(out Vector3 outScale, out Vector3 outPos)
    {
        outScale = _baseScale;
        outPos = _basePosition;

        float maxK = maxGrowMultiplier;
        if (maxK <= 1.001f) return;

        // По каждой оси смотрим свободное место с обеих сторон.
        // Прирост размера делится между сторонами пропорционально свободному месту:
        // стенка вплотную -> эта грань не двигается, блок растёт в другую сторону;
        // свободно с обеих сторон -> растёт симметрично.
        Vector3 shareToPos = Vector3.zero;
        for (int axis = 0; axis < 3; axis++)
        {
            float maxExtra = _baseScale[axis] * (maxK - 1f);
            float freePos = FreeDistance(_basePosition, _baseScale, axis, +1f, maxExtra);
            float freeNeg = FreeDistance(_basePosition, _baseScale, axis, -1f, maxExtra);
            float sum = freePos + freeNeg;
            shareToPos[axis] = sum > 0.0001f ? freePos / sum : 0.5f;
        }

        Vector3 CenterFor(float k)
        {
            Vector3 c = _basePosition;
            for (int axis = 0; axis < 3; axis++)
            {
                float extra = _baseScale[axis] * (k - 1f);
                c[axis] += extra * (2f * shareToPos[axis] - 1f) * 0.5f;
            }
            return c;
        }

        // Ищем максимальный множитель k, при котором блок ещё ни во что не упирается.
        float bestK;
        if (Fits(CenterFor(maxK), _baseScale * maxK))
        {
            bestK = maxK;
        }
        else
        {
            float lo = 1f, hi = maxK;
            for (int i = 0; i < 10; i++)
            {
                float mid = (lo + hi) * 0.5f;
                if (Fits(CenterFor(mid), _baseScale * mid)) lo = mid;
                else hi = mid;
            }
            bestK = lo;
        }

        outScale = _baseScale * bestK;
        outPos = CenterFor(bestK);
    }

    // ---------- Уменьшение ----------

    private void CalculateShrink(out Vector3 outScale, out Vector3 outPos)
    {
        Vector3 newScale = _baseScale * minScaleMultiplier;
        Vector3 newPos = _basePosition;

        // Считаем, сколько граней блока прилегает к поверхностям
        int touches = 0;
        int touchAxis = 0;
        float touchSign = 0f;

        for (int axis = 0; axis < 3; axis++)
        {
            for (int s = -1; s <= 1; s += 2)
            {
                if (FreeDistance(_basePosition, _baseScale, axis, s, touchTolerance) < touchTolerance)
                {
                    touches++;
                    touchAxis = axis;
                    touchSign = s;
                }
            }
        }

        // Ровно одна поверхность -> сжимаемся к ней (эта грань остаётся на месте).
        // Несколько поверхностей или ни одной -> сжимаемся в центр.
        if (touches == 1)
        {
            float shift = (_baseScale[touchAxis] - newScale[touchAxis]) * 0.5f;
            newPos[touchAxis] += touchSign * shift;
        }

        outScale = newScale;
        outPos = newPos;
    }

    // ---------- Физические проверки ----------

    private bool IsOwn(Collider c) => c.transform.IsChildOf(_root);

    /// <summary>Помещается ли бокс такого размера в этом месте (свои коллайдеры не считаются).</summary>
    private bool Fits(Vector3 center, Vector3 size)
    {
        Vector3 half = Vector3.Max(size * 0.5f - Vector3.one * skin, Vector3.one * 0.001f);
        int n = Physics.OverlapBoxNonAlloc(center, half, _overlapBuf, Quaternion.identity,
                                           wallMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < n; i++)
        {
            if (!IsOwn(_overlapBuf[i])) return false;
        }
        return true;
    }

    /// <summary>Свободное расстояние от грани блока до ближайшего препятствия вдоль оси (не больше maxDist).</summary>
    private float FreeDistance(Vector3 pos, Vector3 size, int axis, float sign, float maxDist)
    {
        Vector3 dir = Vector3.zero;
        dir[axis] = sign;

        Vector3 half = Vector3.Max(size * 0.5f - Vector3.one * skin, Vector3.one * 0.001f);
        int n = Physics.BoxCastNonAlloc(pos, half, dir, _hitBuf, Quaternion.identity,
                                        maxDist + skin, wallMask, QueryTriggerInteraction.Ignore);

        float best = maxDist;
        for (int i = 0; i < n; i++)
        {
            if (IsOwn(_hitBuf[i].collider)) continue;
            // бокс уменьшен на skin со всех сторон, поэтому вычитаем skin из расстояния
            best = Mathf.Min(best, Mathf.Max(0f, _hitBuf[i].distance - skin));
        }
        return best;
    }
}