using UnityEngine;
using UnityEngine.InputSystem;

public class SPlayerLensInput : MonoBehaviour
{
    [SerializeField] private LensInventory inventory;
    [SerializeField] private LensData[] quickLenses;

    [SerializeField] private InputActionReference scrollAction;

    private int _currentIndex = -1;

    private void Awake()
    {
        if (inventory == null)
        {
            Debug.LogWarning("Не подключен инвентарь!");
            inventory = FindAnyObjectByType<LensInventory>();
        }
        if (quickLenses == null)
        {
            Debug.LogWarning("Не подключены линзы!");
        }
        if (scrollAction == null)
        {
            Debug.LogError("Не подключен переключатель линз!");
        }
    }

    private void OnEnable()
    {
        scrollAction.action.Enable();
        scrollAction.action.performed += OnScroll;
    }

    private void OnDisable()
    {
        scrollAction.action.Disable();
        scrollAction.action.performed -= OnScroll;
    }

    public void OnScroll(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<float>();
        if (Mathf.Abs(scroll) < 0.01f) return;

        int direction = scroll > 0 ? 1 : -1;
        CycleLens(direction);
    }

    private void CycleLens(int direction)
    {
        if (quickLenses.Length == 0) return;

        _currentIndex += direction;
        // Зацикливание
        if (_currentIndex < 0) _currentIndex = quickLenses.Length - 1;
        if (_currentIndex >= quickLenses.Length) _currentIndex = 0;

        // Снимаем все и надеваем текущую
        inventory.ClearAll();
        inventory.Equip(quickLenses[_currentIndex]);
    }

    private void Toggle(LensData lens)
    {
        if (inventory.IsEquipped(lens))
            inventory.Unequip(lens);
        else
            inventory.Equip(lens);
    }
}