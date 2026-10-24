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
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Проверяем, есть ли на объекте (или его родителе) скрипт YellowMovingBlock
            var yellowBlock = hit.collider.GetComponentInParent<YellowMovingBlock>();
            if (yellowBlock != null)
            {
                // Передаем точку попадания (hit.point) и нормаль грани (hit.normal)
                yellowBlock.Interact(hit.point, hit.normal);
            }
        }
    }
}