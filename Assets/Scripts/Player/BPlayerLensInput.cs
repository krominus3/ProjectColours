using UnityEngine;
using UnityEngine.InputSystem;

public class BPlayerLensInput : MonoBehaviour
{
    [SerializeField] private LensInventory inventory;

    [Header("����� �� �������")]
    [SerializeField] private LensData firstLens;
    [SerializeField] private LensData SecondLens;
    [SerializeField] private LensData ThirdLens;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference firstLensAction;
    [SerializeField] private InputActionReference SecondLensAction;
    [SerializeField] private InputActionReference ThirdLensAction;

    private void Awake()
    {
        if (inventory == null)
        {
            Debug.LogWarning("�� ��������� ���������!");
            inventory = FindAnyObjectByType<LensInventory>();
        }
        if (!firstLensAction || !SecondLensAction || !ThirdLensAction)
        {
            Debug.LogError("�� ���������� ������������ ����!");
        }
    }

    private void OnEnable()
    {
        Subscribe(firstLensAction, OnFirstPressed, firstLens);
        Subscribe(SecondLensAction, OnSecondPressed, SecondLens);
        Subscribe(ThirdLensAction, OnThirdPressed, ThirdLens);
    }

    private void OnDisable()
    {
        Unsubscribe(firstLensAction, OnFirstPressed);
        Unsubscribe(SecondLensAction, OnSecondPressed);
        Unsubscribe(ThirdLensAction, OnThirdPressed);
    }

    private void Subscribe(InputActionReference actionRef, System.Action<InputAction.CallbackContext> handler, LensData lens)
    {
        if (actionRef == null)
        {
            Debug.LogError($"�� ��������� Input Action ��� �����: {(lens != null ? lens.name : "null")}");
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

    // --- ����������� ---

    private void OnFirstPressed(InputAction.CallbackContext ctx) => Toggle(firstLens);
    private void OnSecondPressed(InputAction.CallbackContext ctx) => Toggle(SecondLens);
    private void OnThirdPressed(InputAction.CallbackContext ctx) => Toggle(ThirdLens);

    // --- ������ ---

    private void Toggle(LensData lens)
    {
        if (lens == null || inventory == null) return;

        if (inventory.IsEquipped(lens))
        {
            inventory.Unequip(lens);
        }
        else
        {
            inventory.Equip(lens);
        }
    }
}