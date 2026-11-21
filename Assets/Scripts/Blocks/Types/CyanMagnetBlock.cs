using UnityEngine;

/// <summary>
/// Голубой блок: магнит. Он притягивает или отталкивает магнитные объекты.
/// Вращается вверх через DirectionArrow (не сам по себе, как в других блоках).
/// Притягивает / отталкивает вдоль стрелки (SizeArrowGroup):
///   стрелка наружу = притягивает,
///   стрелка внутрь = отталкивает.
/// Внимание, стрелка может быть отключена.
/// Притягивает объекты с тегом magneticTag, а также игрока (CharacterController).
/// </summary>
public class CyanMagnetBlock : MonoBehaviour
{
    [Header("Параметры магнита")]
    [SerializeField] private float maxDistance = 10f;
    [Tooltip("Сила для объектов с Rigidbody (ForceMode.Force).")]
    [SerializeField] private float magnetForce = 15f;
    [Tooltip("Скорость, с которой игрок тянется / толкается (CharacterController), м/с.")]
    [SerializeField] private float playerSpeed = 8f;
    [SerializeField] private string magneticTag = "Magnetic";
    [Tooltip("Применять ли эффект к игроку (в режиме Player, а не Magnetic, который притягивает всё).")]
    [SerializeField] private bool affectPlayer = true;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Стрелка")]
    [Tooltip("Стрелка направления (та же, что и у жёлтого блока). Если её нет — берётся из корня блока автоматически. " +
             "Для магнита важна как раз она (transform.forward может отличаться).")]
    [SerializeField] private DirectionArrow directionArrow;
    [Tooltip("Группа стрелок размера. Если нет — ищется в корне блока автоматически. " +
             "Наружу = притягивает, внутрь = отталкивает.")]
    [SerializeField] private SizeArrowGroup arrowGroup;
    [Tooltip("Изначально, тянет блок к себе или толкает от себя.")]
    [SerializeField] private bool isAttracting = true;

    [Header("Луч")]
    [SerializeField] private LineRenderer lineRenderer;

    // Небольшое смещение вниз, чтобы компенсировать потерю контакта с землёй
    // CharacterController.isGrounded (он теряется во время вызова Move).
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

        Vector3 dir = directionArrow.ForwardDirection;
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

    /// <summary>Ближайшее препятствие на пути луча. Если попадание в триггер — игнорируем его и идём дальше.</summary>
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
        // dir — от магнита к цели. Притягиваем — в сторону магнита (-dir), отталкиваем — от магнита (+dir).
        Vector3 pull = Attracting ? -dir : dir;

        // Магнитные объекты
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