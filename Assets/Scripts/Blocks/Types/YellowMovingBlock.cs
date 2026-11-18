using UnityEngine;

public class YellowMovingBlock : MonoBehaviour
{
    [Header("Параметры движения")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask wallMask = ~0;

    [Header("Параметры толкания (без стрелки)")]
    [SerializeField] private float pushDistance = 1f;
    [SerializeField] private float inputBufferTime = 0.5f;

    [Header("Стрелка")]
    [Tooltip("Ссылка на стрелку (DirectionArrow).")]
    [SerializeField] private DirectionArrow arrow;

    private ColoredBlock _block;
    private Transform _rootObject;

    private bool _isMoving = false;
    private Vector3 _moveDirection;
    private Vector3 _targetPosition;

    // Буфер нажатия
    private Vector3 _bufferedDirection;
    private bool _hasBufferedInput = false;
    private float _bufferTimer = 0f;

    // Проверяем, есть ли активная стрелка
    private bool hasArrow => arrow != null && arrow.isActive;

    private bool IsYellow => _block == null || _block.currentColor == LensType.Yellow;

    private void Awake()
    {
        _block = GetComponentInParent<ColoredBlock>();
        _rootObject = _block != null ? _block.transform : transform;
        _targetPosition = _rootObject.position;
    }

    private void OnEnable()
    {
        if (arrow != null) arrow.Interacted += OnArrowInteracted;
    }

    private void OnDisable()
    {
        if (arrow != null) arrow.Interacted -= OnArrowInteracted;
    }

    // Стрелка повёрнута игроком. Вращается она при ЛЮБОМ цвете блока.
    // Ехать блок начнёт сразу, только если он сейчас жёлтый;
    // иначе подхватит направление, когда станет жёлтым (см. Update).
    private void OnArrowInteracted(BlockArrow _)
    {
        if (!hasArrow || !IsYellow) return;

        _moveDirection = arrow.ForwardDirection;
        _targetPosition = _rootObject.position + _moveDirection * 100f;
        _isMoving = true;
    }

    private void Update()
    {
        if (!IsYellow) return;

        // Со стрелкой блок едет сам (постоянное движение)
        if (hasArrow)
        {
            if (!_isMoving && _targetPosition == _rootObject.position)
            {
                _moveDirection = arrow.ForwardDirection;
                _isMoving = true;
            }
        }

        // Движение
        if (_isMoving)
        {
            MoveBlock();
        }
        // Гравитация (только без стрелки)
        else if (!hasArrow)
        {
            ApplyGravity();
        }

        // Обработка буфера нажатия
        if (_hasBufferedInput)
        {
            _bufferTimer -= Time.deltaTime;

            if (_bufferTimer <= 0f)
            {
                _hasBufferedInput = false;
            }
            else if (!_isMoving && !hasArrow)
            {
                ExecutePush(_bufferedDirection);
                _hasBufferedInput = false;
            }
        }
    }

    public void Interact(Vector3 playerLookDirection, Vector3 hitPoint)
    {
        // Со стрелкой: игрок нажал на сам блок. Стрелка реагирует на это, только если
        // у неё разрешено «любая часть блока». Нажатие прямо на стрелку приходит
        // отдельно, через событие Interacted (см. OnArrowInteracted).
        if (hasArrow)
        {
            if (arrow.AcceptsAnyPartOfBlock) arrow.Interact();
            return;
        }

        // Без стрелки: толкание работает только у жёлтого блока
        if (!IsYellow) return;

        Vector3 flatDir = new Vector3(playerLookDirection.x, 0, playerLookDirection.z).normalized;
        Vector3 snapDir;
        if (Mathf.Abs(flatDir.x) > Mathf.Abs(flatDir.z))
            snapDir = new Vector3(Mathf.Sign(flatDir.x), 0, 0);
        else
            snapDir = new Vector3(0, 0, Mathf.Sign(flatDir.z));

        if (_isMoving)
        {
            _bufferedDirection = snapDir;
            _hasBufferedInput = true;
            _bufferTimer = inputBufferTime;
        }
        else
        {
            ExecutePush(snapDir);
        }
    }

    private void ExecutePush(Vector3 direction)
    {
        _moveDirection = direction;
        _targetPosition = _rootObject.position + _moveDirection * pushDistance;
        _isMoving = true;
    }

    private void MoveBlock()
    {
        Vector3 halfExtents = _rootObject.localScale / 2f;
        halfExtents *= 0.95f;
        float castDistance = moveSpeed * Time.deltaTime + 0.05f;

        if (Physics.BoxCast(_rootObject.position, halfExtents, _moveDirection, out RaycastHit hit, _rootObject.rotation, Mathf.Max(castDistance, 0.1f), wallMask))
        {
            _isMoving = false;
            _targetPosition = _rootObject.position;

            // Важно: сохранить базовое состояние для синего блока
            var blueBlock = GetComponent<BlueSizeBlock>();
            if (blueBlock != null) blueBlock.SaveBaseTransform();
            return;
        }

        if (hasArrow)
        {
            _rootObject.position += _moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            _rootObject.position = Vector3.MoveTowards(_rootObject.position, _targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(_rootObject.position, _targetPosition) < 0.01f)
            {
                _rootObject.position = _targetPosition;
                _isMoving = false;

                // Важно: сохранить новую позицию для BlueSizeBlock
                var blueBlock = GetComponent<BlueSizeBlock>();
                if (blueBlock != null) blueBlock.SaveBaseTransform();
            }
        }
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