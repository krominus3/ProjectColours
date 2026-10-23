using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerDash : MonoBehaviour
{
    [Header("Настройки рывка")]
    [SerializeField] private float dashSpeed = 20f;      // Скорость рывка
    [SerializeField] private float dashDuration = 0.2f;  // Длительность (сек)
    [SerializeField] private float dashCooldown = 1f;    // Перезарядка (сек)

    [Header("Ссылки")]
    [SerializeField] private Transform cameraTransform;  // Ссылка на камеру
    [SerializeField] private InputActionReference dashAction;

    private CharacterController _controller;
    private bool _canDash = false; // По умолчанию рывка нет
    private bool _isDashing = false;
    private float _dashTimeLeft;
    private float _cooldownTimeLeft;
    private Vector3 _dashDirection;

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
                _canDash = true;
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
                // Двигаем игрока в направлении рывка
                _controller.Move(_dashDirection * dashSpeed * Time.deltaTime);
            }
        }
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (!_canDash || _isDashing || _cooldownTimeLeft > 0f) return;

        // 1. Сбрасываем вертикальную скорость в PlayerMovement
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ResetVerticalVelocity();
        }

        // 2. Запускаем рывок
        _isDashing = true;
        _canDash = false;
        _dashTimeLeft = dashDuration;

        // Вычисляем направление (куда смотрит камера)
        Vector3 forward = cameraTransform.forward;

        // Берем направление взгляда
        Vector3 dir = forward;

        // Ограничение по вертикали (не больше 45 градусов)
        // Если смотрим слишком вверх или вниз, обнуляем Y (делаем рывок горизонтальным)
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

    // Метод, который будет вызывать пурпурный блок
    public void AddDash()
    {
        _canDash = true;
        _cooldownTimeLeft = 0f; // Сбрасываем кулдаун при получении
        Debug.Log("[PlayerDash] Получен рывок!");
    }
    
    public void RemoveDash()
    {
        _canDash = false;
        _cooldownTimeLeft = 0f; // Сбрасываем кулдаун при получении
        Debug.Log("[PlayerDash] Отобран рывок!");
    }
}