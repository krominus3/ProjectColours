using UnityEngine;

public class YellowMovingBlock : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private bool hasArrow = true;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask wallMask = ~0;

    [Header("Визуал стрелки")]
    [SerializeField] private Transform arrowPivot;
    [Tooltip("Радиус зоны вокруг центра, куда нужно смотреть, чтобы повернуть стрелку")]
    [SerializeField] private float arrowHitRadius = 0.5f;

    private ColoredBlock _block;
    private Transform _rootObject;

    private bool _isMoving = false;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _block = GetComponentInParent<ColoredBlock>();
        _rootObject = _block != null ? _block.transform : transform;
    }

    private void Update()
    {
        if (_block != null && _block.currentColor != LensType.Yellow) return;

        if (hasArrow)
        {
            if (!_isMoving)
            {
                _moveDirection = arrowPivot.forward;
                _isMoving = true;
            }
        }

        if (_isMoving)
        {
            MoveTowardsWall();
        }
        else if (!hasArrow)
        {
            ApplyGravity();
        }
    }

    public void Interact(Vector3 playerLookDirection, Vector3 hitPoint)
    {
        if (_block != null && _block.currentColor != LensType.Yellow) return;

        if (hasArrow)
        {
            // Проверяем, попал ли игрок в зону стрелки (в центр)
            float distanceToCenter = Vector3.Distance(hitPoint, arrowPivot.position);

            if (distanceToCenter <= arrowHitRadius)
            {
                // Игрок смотрит в центр — крутим стрелку
                arrowPivot.Rotate(0, 90f, 0, Space.Self);
                _moveDirection = arrowPivot.forward;
                _isMoving = true;
                Debug.Log("[YellowBlock] Стрелка повернута!");
            }
            else
            {
                Debug.Log("[YellowBlock] Игрок нажал мимо стрелки.");
            }
        }
        else
        {
            // Толкаем от игрока по 4 сторонам (для блока без стрелки)
            Vector3 flatDir = new Vector3(playerLookDirection.x, 0, playerLookDirection.z).normalized;
            Vector3 snapDir = Vector3.zero;
            if (Mathf.Abs(flatDir.x) > Mathf.Abs(flatDir.z))
                snapDir = new Vector3(Mathf.Sign(flatDir.x), 0, 0);
            else
                snapDir = new Vector3(0, 0, Mathf.Sign(flatDir.z));

            _moveDirection = snapDir;
            _isMoving = true;
        }
    }

    private void MoveTowardsWall()
    {
        Vector3 halfExtents = _rootObject.localScale / 2f;
        halfExtents *= 0.95f;
        float castDistance = moveSpeed * Time.deltaTime + 0.05f;

        bool hasHit = Physics.BoxCast(
            _rootObject.position,
            halfExtents,
            _moveDirection,
            out RaycastHit hit,
            _rootObject.rotation,
            castDistance,
            wallMask
        );

        if (hasHit)
        {
            float safeDistance = (hit.distance - castDistance);
            if (safeDistance > 0) _rootObject.position += _moveDirection * safeDistance;

            _isMoving = false;
            return;
        }

        _rootObject.position += _moveDirection * moveSpeed * Time.deltaTime;
    }

    private void ApplyGravity()
    {
        float halfHeight = _rootObject.localScale.y / 2f;
        if (!Physics.Raycast(_rootObject.position, Vector3.down, halfHeight + 0.05f, wallMask))
        {
            _rootObject.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
    }
}