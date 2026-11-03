using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerDash : MonoBehaviour
{
    [Header("��������� �����")]
    [SerializeField] private float dashSpeed = 20f;      // �������� �����
    [SerializeField] private float dashDuration = 0.2f;  // ������������ (���)
    [SerializeField] private float dashCooldown = 1f;    // ����������� (���)

    [Header("������")]
    [SerializeField] private Transform cameraTransform;  // ������ �� ������
    [SerializeField] private InputActionReference dashAction;

    private CharacterController _controller;
    private bool _canDash = false; // �� ��������� ����� ���
    private bool _isDashing = false;
    private float _dashTimeLeft;
    private float _cooldownTimeLeft;
    private Vector3 _dashDirection;

    private bool _CanDash
    {
        get => _canDash;
        set
        {
            _canDash = value;
            OnDashAbilityChanged?.Invoke(_canDash);
        }
    }

    public event Action<bool> OnDashAbilityChanged;
    
    public bool HasDash => _CanDash;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        dashAction.action.Enable();
        dashAction.action.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        dashAction.action.Disable();
        dashAction.action.performed -= OnDashPerformed;
    }

    private void Update()
    {
        if (_cooldownTimeLeft > 0f)
        {
            _cooldownTimeLeft -= Time.deltaTime;
            if (_cooldownTimeLeft <= 0f && !_isDashing)
            {
                //_CanDash = true;
            }
        }

        if (_isDashing)
        {
            _dashTimeLeft -= Time.deltaTime;
            if (_dashTimeLeft <= 0f)
            {
                _isDashing = false;
                _cooldownTimeLeft = dashCooldown;
            }
            else
            {
                // ������� ������ � ����������� �����
                _controller.Move(_dashDirection * dashSpeed * Time.deltaTime);
            }
        }
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (!_CanDash || _isDashing || _cooldownTimeLeft > 0f) return;

        // 1. ���������� ������������ �������� � PlayerMovement
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ResetVerticalVelocity();
        }

        // 2. ��������� �����
        _isDashing = true;
        _CanDash = false;
        _dashTimeLeft = dashDuration;

        // ��������� ����������� (���� ������� ������)
        Vector3 forward = cameraTransform.forward;

        // ����� ����������� �������
        Vector3 dir = forward;

        // ����������� �� ��������� (�� ������ 45 ��������)
        // ���� ������� ������� ����� ��� ����, �������� Y (������ ����� ��������������)
        if (dir.y > 0.7f)
        {
            dir.y = 0.7f;
        }
        else if (dir.y < -0.7f)
        {
            dir.y = -0.7f;
        }
        else
        {
            dir.y = 0f;
        }

        _dashDirection = dir.normalized;
    }

    // �����, ������� ����� �������� ��������� ����
    public void AddDash()
    {
        _CanDash = true;
        _cooldownTimeLeft = 0f; // ���������� ������� ��� ���������
        Debug.Log("[PlayerDash] ������� �����!");
    }
    
    public void RemoveDash()
    {
        _CanDash = false;
        _cooldownTimeLeft = 0f; // ���������� ������� ��� ���������
        Debug.Log("[PlayerDash] ������� �����!");
    }
}