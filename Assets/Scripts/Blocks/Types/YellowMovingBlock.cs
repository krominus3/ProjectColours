using UnityEngine;

public class YellowMovingBlock : MonoBehaviour
{
    [Header("Ïàðàìåòðû äâèæåíèÿ")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask wallMask = ~0;

    [Header("Ïàðàìåòðû òîëêàíèÿ (áåç ñòðåëêè)")]
    [SerializeField] private float pushDistance = 1f;
    [SerializeField] private float inputBufferTime = 0.5f;

    [Header("Ñòðåëêà")]
    [Tooltip("Ññûëêà íà ñòðåëêó (DirectionArrow).")]
    [SerializeField] private DirectionArrow arrow;

    private ColoredBlock _block;
    private Transform _rootObject;

    private bool _isMoving = false;
    private Vector3 _moveDirection;
    private Vector3 _targetPosition;

    // Áóôåð íàæàòèÿ
    private Vector3 _bufferedDirection;
    private bool _hasBufferedInput = false;
    private float _bufferTimer = 0f;

    // Ïðîâåðÿåì, åñòü ëè àêòèâíàÿ ñòðåëêà
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

    // Ñòðåëêà ïîâ¸ðíóòà èãðîêîì. Âðàùàåòñÿ îíà ïðè ËÞÁÎÌ öâåòå áëîêà.
    // Åõàòü áëîê íà÷í¸ò ñðàçó, òîëüêî åñëè îí ñåé÷àñ æ¸ëòûé;
    // èíà÷å ïîäõâàòèò íàïðàâëåíèå, êîãäà ñòàíåò æ¸ëòûì (ñì. Update).
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

        // Ñî ñòðåëêîé áëîê åäåò ñàì (ïîñòîÿííîå äâèæåíèå)
        if (hasArrow)
        {
            if (!_isMoving && _targetPosition == _rootObject.position)
            {
                _moveDirection = arrow.ForwardDirection;
                _isMoving = true;
            }
        }

        // Äâèæåíèå
        if (_isMoving)
        {
            MoveBlock();
        }
        // Ãðàâèòàöèÿ (òîëüêî áåç ñòðåëêè)
        else if (!hasArrow)
        {
            ApplyGravity();
        }

        // Îáðàáîòêà áóôåðà íàæàòèÿ
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
        // Ñî ñòðåëêîé: èãðîê íàæàë íà ñàì áëîê. Ñòðåëêà ðåàãèðóåò íà ýòî, òîëüêî åñëè
        // ó íå¸ ðàçðåøåíî «ëþáàÿ ÷àñòü áëîêà». Íàæàòèå ïðÿìî íà ñòðåëêó ïðèõîäèò
        // îòäåëüíî, ÷åðåç ñîáûòèå Interacted (ñì. OnArrowInteracted).
        if (hasArrow)
        {
            if (arrow.AcceptsAnyPartOfBlock) arrow.Interact();
            return;
        }

        // Áåç ñòðåëêè: òîëêàíèå ðàáîòàåò òîëüêî ó æ¸ëòîãî áëîêà
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

            // Âàæíî: ñîõðàíèòü áàçîâîå ñîñòîÿíèå äëÿ ñèíåãî áëîêà
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

                // Âàæíî: ñîõðàíèòü íîâóþ ïîçèöèþ äëÿ BlueSizeBlock
                var blueBlock = GetComponent<BlueSizeBlock>();
                if (blueBlock != null) blueBlock.SaveBaseTransform();
            }
        }
    }

    private const float GroundSkin = 0.02f;
    private readonly RaycastHit[] _groundHits = new RaycastHit[8];

    private void ApplyGravity()
    {
        // Â ïåðâûå êàäðû ïîñëå çàãðóçêè ñöåíû deltaTime áûâàåò áîëüøèì, îãðàíè÷èâàåì øàã
        float step = moveSpeed * Mathf.Min(Time.deltaTime, 0.05f);

        // Òîíêèé áîêñ ïî ðàçìåðó íèæíåé ãðàíè áëîêà, ÷óòü âûøå ñàìîé ãðàíè (÷òîáû íå
        // ïåðåñåêàòü ïîë, íà êîòîðîì áëîê ñòîèò). Ñìîòðèì, ñêîëüêî ñâîáîäíîãî ìåñòà äî ïîëà.
        Vector3 scale = _rootObject.localScale;
        Vector3 half = new Vector3(scale.x * 0.5f * 0.95f, 0.02f, scale.z * 0.5f * 0.95f);
        Vector3 origin = _rootObject.position + Vector3.down * (scale.y * 0.5f - GroundSkin - half.y);

        int count = Physics.BoxCastNonAlloc(origin, half, Vector3.down, _groundHits, _rootObject.rotation,
                                            step + GroundSkin, wallMask, QueryTriggerInteraction.Ignore);

        // Ïàäàåì íå äàëüøå ïîëà: ïîñëåäíèé øàã óêîðà÷èâàåòñÿ, è áëîê âñòà¸ò ðîâíî íà ïîâåðõíîñòü
        float fall = step;
        for (int i = 0; i < count; i++)
        {
            if (_groundHits[i].collider.transform.IsChildOf(_rootObject)) continue;
            fall = Mathf.Min(fall, Mathf.Max(0f, _groundHits[i].distance - GroundSkin));
        }

        if (fall > 0.0005f)
            _rootObject.position += Vector3.down * fall;

        // Áëîê «ñòîèò», öåëü äâèæåíèÿ = òåêóùàÿ ïîçèöèÿ (èíà÷å ïîñëå ïàäåíèÿ ñòðåëêà,
        // âêëþ÷¸ííàÿ ïîçæå, íå ñìîãëà áû çàïóñòèòü áëîê: ñì. Update)
        _targetPosition = _rootObject.position;
    }
}