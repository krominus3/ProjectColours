using UnityEngine;

public class YellowMovingBlock : MonoBehaviour
{
    [Header("��������� ��������")]
    [SerializeField] private bool hasArrow = true;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveDistance = 2f; // ��� ��������

    private Vector3 _targetPos;
    private bool _isMoving = false;

    private ColoredBlock _block;
    private Transform _rootObject;

    private void Awake()
    {
        _block = GetComponentInParent<ColoredBlock>();
        _rootObject = _block != null ? _block.transform : transform;

        // ���������� ���� � ���, ��� �� �����
        _targetPos = _rootObject.position;
    }

    private void Update()
    {
        if (_block != null && _block.currentColor != LensType.Yellow) return;

        if (_isMoving)
        {
            _rootObject.position = Vector3.MoveTowards(_rootObject.position, _targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(_rootObject.position, _targetPos) < 0.01f)
            {
                _isMoving = false;
                // �����: ��� ������ �������, ���������� ���� �� ������� �������
                _targetPos = _rootObject.position;
            }
        }
    }

    public void Interact(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (_isMoving) return;

        if (hasArrow)
        {
            // �����: ����� ������� �������, � �� ���������!
            _targetPos = _rootObject.position + _rootObject.up * moveDistance;
            _isMoving = true;
        }
        else
        {
            // ������� � ������� �� ������
            Vector3 pushDirection = -hitNormal;
            _targetPos = _rootObject.position + pushDirection * moveDistance;
            _isMoving = true;
        }
    }
}