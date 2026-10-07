using UnityEngine;
using UnityEngine.InputSystem;

public class BPlayerLensInput : MonoBehaviour
{
    [SerializeField] private LensInventory inventory;

    [Header("Линзы на клавиши")]
    [SerializeField] private LensData redLens;
    [SerializeField] private LensData blueLens;
    [SerializeField] private LensData yellowLens;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference redLensAction;
    [SerializeField] private InputActionReference blueLensAction;
    [SerializeField] private InputActionReference yellowLensAction;

    private void Awake()
    {
        if (inventory == null)
        {
            Debug.LogWarning("Не подключен инвентарь!");
            inventory = FindAnyObjectByType<LensInventory>();
        }
        if (redLensAction || blueLensAction || yellowLensAction)
        {
            Debug.LogError("Не подключено переключение линз!");
        }
    }

    private void OnEnable()
    {
        Subscribe(redLensAction, OnRedPressed, redLens);
        Subscribe(blueLensAction, OnBluePressed, blueLens);
        Subscribe(yellowLensAction, OnYellowPressed, yellowLens);
    }

    private void OnDisable()
    {
        Unsubscribe(redLensAction, OnRedPressed);
        Unsubscribe(blueLensAction, OnBluePressed);
        Unsubscribe(yellowLensAction, OnYellowPressed);
    }

    private void Subscribe(InputActionReference actionRef, System.Action<InputAction.CallbackContext> handler, LensData lens)
    {
        if (actionRef == null)
        {
            Debug.LogError($"Не подключен Input Action для линзы: {(lens != null ? lens.name : "null")}");
            return;
        }
        actionRef.action.Enable();
        actionRef.action.performed += handler;
    }

    private void Unsubscribe(InputActionReference actionRef, System.Action<InputAction.CallbackContext> handler)
    {
        if (actionRef == null) return;
        actionRef.action.performed -= handler;
        actionRef.action.Disable();
    }

    // --- Обработчики ---

    private void OnRedPressed(InputAction.CallbackContext ctx) => Toggle(redLens);
    private void OnBluePressed(InputAction.CallbackContext ctx) => Toggle(blueLens);
    private void OnYellowPressed(InputAction.CallbackContext ctx) => Toggle(yellowLens);

    // --- Логика ---

    private void Toggle(LensData lens)
    {
        if (lens == null || inventory == null) return;

        if (inventory.IsEquipped(lens))
            inventory.Unequip(lens);
        else
            inventory.Equip(lens);
    }
}