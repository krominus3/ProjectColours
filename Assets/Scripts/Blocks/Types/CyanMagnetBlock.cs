using UnityEngine;

/// <summary>
/// Бирюзовый цвет: магнит. Луч притягивает или отталкивает «магнитные» объекты.
/// Направление луча задаёт DirectionArrow (та же стрелка, что у жёлтого блока).
/// Притяжение / отталкивание задают угловые стрелки (SizeArrowGroup):
///   стрелки внутрь  = притяжение,
///   стрелки наружу  = отталкивание.
/// Работает, только пока блок бирюзовый.
/// Магнитными считаются объекты с тегом magneticTag, а также игрок (CharacterController).
/// </summary>
public class CyanMagnetBlock : MonoBehaviour
{
    [Header("Параметры магнита")]
    [SerializeField] private float maxDistance = 10f;
    [Tooltip("Сила для объектов с Rigidbody (ForceMode.Force).")]
    [SerializeField] private float magnetForce = 15f;
    [Tooltip("Скорость, с которой магнит тянет / толкает игрока (CharacterController), ед/сек.")]
    [SerializeField] private float playerSpeed = 8f;
    [SerializeField] private string magneticTag = "Magnetic";
    [Tooltip("Действует ли магнит на игрока (у игрока тег Player, а не Magnetic, поэтому отдельная опция).")]
    [SerializeField] private bool affectPlayer = true;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Стрелки")]
    [Tooltip("Стрелка направления (та же, что у жёлтого блока). Если не задана — ищется среди дочерних объектов. " +
             "Без активной стрелки луч идёт вперёд (transform.forward этого объекта).")]
    [SerializeField] private DirectionArrow directionArrow;
    [Tooltip("Группа угловых стрелок. Если пусто — ищется среди дочерних объектов. " +
             "Внутрь = притяжение, наружу = отталкивание.")]
    [SerializeField] private SizeArrowGroup arrowGroup;
    [Tooltip("Используется, только если на блоке нет группы угловых стрелок.")]
    [SerializeField] private bool isAttracting = true;

    [Header("Визуал")]
    [SerializeField] private LineRenderer lineRenderer;

    // Небольшое прижатие к земле, чтобы горизонтальные толчки магнита не сбивали
    // CharacterController.isGrounded (он берётся из последнего вызова Move).
    private const float GroundStick = 0.01f;

    private ColoredBlock _block;
    private Transform _root;
    private readonly RaycastHit[] _hits = new RaycastHit[16];

    private bool IsCyan => _block == null || _block.currentColor == LensType.Cyan;
    private bool Attracting => arrowGroup != null ? !arrowGroup.IsOutward : isAttracting;

    private Vector3 BeamDirection =>
        directionArrow != null && directionArrow.isActive
            ? directionArrow.ForwardDirection
            : transform.forward;

    private void Awake()
    {
        _block = GetComponentInParent<ColoredBlock>();
        _root = _block != null ? _block.transform : transform;

        if (directionArrow == null)
            directionArrow = _root.GetComponentInChildren<DirectionArrow>(true);
        if (arrowGroup == null)
            arrowGroup = _root.GetComponentInChildren<SizeArrowGroup>(true);

        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = 2;
        }
    }

    private void OnDisable()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        bool active = IsCyan;
        if (lineRenderer != null) lineRenderer.enabled = active;
        if (!active) return;

        Vector3 dir = BeamDirection;
        Vector3 origin = _root.position;
        Vector3 end = origin + dir * maxDistance;

        bool hasHit = CastBeam(origin, dir, out RaycastHit hit);
        if (hasHit)
        {
            end = hit.point;
            if (IsMagnetic(hit.collider)) ApplyForce(hit, dir);
        }

        Debug.DrawLine(origin, end, hasHit ? Color.red : Color.green);

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, end);
        }
    }

    /// <summary>Ближайшее препятствие на пути луча. Свои коллайдеры блока и триггеры не в счёт.</summary>
    private bool CastBeam(Vector3 origin, Vector3 dir, out RaycastHit best)
    {
        best = default;
        int count = Physics.RaycastNonAlloc(origin, dir, _hits, maxDistance, hitMask,
                                            QueryTriggerInteraction.Ignore);
        float bestDistance = float.MaxValue;
        bool found = false;

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].collider.transform.IsChildOf(_root)) continue;

            if (_hits[i].distance < bestDistance)
            {
                bestDistance = _hits[i].distance;
                best = _hits[i];
                found = true;
            }
        }
        return found;
    }

    private bool IsMagnetic(Collider col)
    {
        if (col.CompareTag(magneticTag)) return true;
        return affectPlayer && col.GetComponentInParent<CharacterController>() != null;
    }

    private void ApplyForce(RaycastHit hit, Vector3 dir)
    {
        // dir — от магнита к цели. Притяжение — к магниту (-dir), отталкивание — от него (+dir).
        Vector3 pull = Attracting ? -dir : dir;

        // Физические объекты
        if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
        {
            hit.rigidbody.AddForce(pull * magnetForce, ForceMode.Force);
            return;
        }

        // Игрок на CharacterController
        var controller = hit.collider.GetComponentInParent<CharacterController>();
        if (controller != null && controller.enabled)
        {
            Vector3 move = pull * playerSpeed * Time.fixedDeltaTime;
            if (controller.isGrounded) move += Vector3.down * GroundStick;
            controller.Move(move);
        }
    }
}