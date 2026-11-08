using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private InputActionReference interactAction;

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, ~0, QueryTriggerInteraction.Collide))
        {
            var yellowBlock = hit.collider.GetComponentInParent<YellowMovingBlock>();

            if (yellowBlock != null)
            {
                // Передаем и направление взгляда, и точку, куда попал луч
                yellowBlock.Interact(transform.forward, hit.point);
            }
        }
    }
}