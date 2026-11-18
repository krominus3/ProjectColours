using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private InputActionReference interactAction;
    [Tooltip("Насколько глубоко за поверхностью блока ещё засчитывается коллайдер стрелки " +
             "(на случай, если он утоплен в грань).")]
    [SerializeField] private float arrowDepthTolerance = 0.1f;

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

        // 1. Ближайшая твёрдая поверхность: через неё взаимодействовать нельзя.
        //    Свои коллайдеры игрока, стрелки и ЛЮБЫЕ триггеры (в том числе большой Trigger блока,
        //    плиты, лазеры, зоны) не мешают.
        float solidDistance = interactDistance;
        int solidIndex = -1;
        for (int i = 0; i < count; i++)
        {
            Collider col = _hits[i].collider;
            if (col.transform.IsChildOf(transform)) continue;
            if (col.GetComponentInParent<BlockArrow>() != null) continue;
            if (col.isTrigger) continue;

            solidDistance = _hits[i].distance;
            solidIndex = i;
            break;
        }

        // 2. Стрелка, в которую целится игрок (ближайшая из тех, что не за стеной)
        for (int i = 0; i < count; i++)
        {
            if (_hits[i].distance > solidDistance + arrowDepthTolerance) break;

            Collider col = _hits[i].collider;
            if (col.transform.IsChildOf(transform)) continue;

            var arrow = col.GetComponentInParent<BlockArrow>();
            if (arrow != null && arrow.isActive)
            {
                arrow.Interact();
                return;
            }
        }

        // 3. Сам блок (толкание жёлтого блока без стрелки и т.п.)
        if (solidIndex < 0) return;

        // Логика цвета может висеть на дочернем объекте Trigger, поэтому ищем её от корня блока
        Collider solid = _hits[solidIndex].collider;
        var yellowBlock = solid.GetComponentInParent<YellowMovingBlock>();
        if (yellowBlock == null)
        {
            var coloredBlock = solid.GetComponentInParent<ColoredBlock>();
            if (coloredBlock != null)
                yellowBlock = coloredBlock.GetComponentInChildren<YellowMovingBlock>(true);
        }

        if (yellowBlock != null)
            yellowBlock.Interact(transform.forward, _hits[solidIndex].point);
    }
}