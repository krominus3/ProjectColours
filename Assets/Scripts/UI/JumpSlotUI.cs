using UnityEngine;
using UnityEngine.UI;

public class JumpSlotUI : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Image slot;
    [SerializeField] private Image icon;

    [Header("Colours")]
    [SerializeField] private Color activeColor   = new Color32(22,140,67,255);
    [SerializeField] private Color inactiveColor = new Color32(50,50,50,255);

    private void Awake()
    {
        if (slot == null) slot = GetComponent<Image>();
        if (icon == null) Debug.LogError("No icon assigned");
        if (movement == null) movement = FindAnyObjectByType<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (movement != null) movement.OnExtraJumpChanged += HandleChanged;
        Refresh(movement != null && movement._ExtraJump);
    }

    private void OnDisable()
    {
        if (movement != null) movement.OnExtraJumpChanged -= HandleChanged;
    }

    private void HandleChanged(bool has) => Refresh(has);

    private void Refresh(bool has)
    {
        if (slot != null) slot.color = has ? activeColor : inactiveColor;
        if (icon != null) icon.color = has ? activeColor : inactiveColor;
    }
}