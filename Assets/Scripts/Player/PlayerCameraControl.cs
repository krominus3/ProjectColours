using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraControl : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Transform playerBody;   
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private InputActionReference lookAction;


    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f; 
    [SerializeField] private bool lockCursor = true;

    private float _xRotation = 0f;


    private void OnEnable() => lookAction.action.Enable();
    private void OnDisable() => lookAction.action.Disable();

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        playerBody.Rotate(Vector3.up * mouseX);

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);
        cameraPivot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}
