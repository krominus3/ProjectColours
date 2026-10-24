using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("PlayerSettings")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("InputsControllers")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Platform Settings")]
    [SerializeField] private float groundCheckDistance = 0.2f; // Длина луча вниз
    [SerializeField] private LayerMask groundMask = ~0;        // Слои, которые считаем полом

    private MovingPlatform _currentPlatform;

    private Vector3 velocity;
    private bool isGrounded;

    // Счётчик дополнительных прыжков
    private bool _extraJump = false;

    private void Awake()
    {
        if (controller == null)
        {
            Debug.LogWarning("Не подключен CharacterController!");
            controller = FindAnyObjectByType<CharacterController>();
        }
        if (moveAction == null || jumpAction == null)
        {
            Debug.LogError("Не подключены Input Actions!");
        }
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();

        jumpAction.action.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        jumpAction.action.performed -= OnJumpPerformed;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            if (velocity.y < 0) velocity.y = -2f;
            // При приземлении сбрасываем все доп. прыжки
            _extraJump = false;
        }

        CheckForPlatform();
        if (_currentPlatform != null && isGrounded)
        {
            // Двигаем игрока на ту же дельту, на которую сдвинулась платформа
            controller.Move(_currentPlatform.DeltaPosition);
        }

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void CheckForPlatform()
    {
        // Пускаем луч вниз из центра капсулы игрока
        Vector3 origin = transform.position + controller.center;
        // Чуть-чуть не доходя до пола
        float distance = (controller.height / 2f) + groundCheckDistance;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, distance, groundMask))
        {
            // Ищем скрипт MovingPlatform на том, на чем мы стоим
            _currentPlatform = hit.collider.GetComponentInParent<MovingPlatform>();
        }
        else
        {
            _currentPlatform = null;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            // Обычный прыжок
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else if (_extraJump == true)
        {
            // Дополнительный прыжок (зелёный блок)
            _extraJump = false;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // Метод, который будет вызывать зелёный блок
    public void AddExtraJump()
    {
        _extraJump = true;
    }

    public void ResetVerticalVelocity()
    {
        velocity.y = 0f;
    }
}