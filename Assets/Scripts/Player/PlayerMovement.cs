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

    private Vector3 velocity;
    private bool isGrounded;

    // Счётчик дополнительных прыжков
    private int _extraJumps = 0;

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
            _extraJumps = 0;
        }

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            // Обычный прыжок
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else if (_extraJumps > 0)
        {
            // Дополнительный прыжок (зелёный блок)
            _extraJumps--;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // Метод, который будет вызывать зелёный блок
    public void AddExtraJump(int amount = 1)
    {
        _extraJumps += amount;
    }
}