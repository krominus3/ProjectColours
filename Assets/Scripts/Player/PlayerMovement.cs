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

    private void Awake()
    {
        if (controller == null)
        {
            Debug.LogWarning("Не подключен CharacterController!");
            controller = FindAnyObjectByType<CharacterController>();
        }
        if (moveAction == null || jumpAction == null)
        {
            Debug.LogError("Не подключено передвижение персонажем!");
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

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
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
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

}