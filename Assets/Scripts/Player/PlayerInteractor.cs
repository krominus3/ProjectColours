using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private InputActionReference interactAction;

    private readonly RaycastHit[] _hits = new RaycastHit[16];
    private static readonly Comparer<RaycastHit> ByDistance =
        Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance));

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

        int count = Physics.RaycastNonAlloc(ray, _hits, interactDistance, ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(_hits, 0, count, ByDistance);

        for (int i = 0; i < count; i++)
        {
            Collider col = _hits[i].collider;

            // Свои коллайдеры игрока луч не блокируют
            if (col.transform.IsChildOf(transform)) continue;

            var yellowBlock = col.GetComponentInParent<YellowMovingBlock>();
            if (yellowBlock != null)
            {
                yellowBlock.Interact(transform.forward, _hits[i].point);
                return;
            }

            // Триггеры (нажимные плиты, лазеры, зоны) луч не блокируют,
            // а любая твёрдая поверхность не даёт взаимодействовать сквозь себя
            if (!col.isTrigger) return;
        }
    }
}