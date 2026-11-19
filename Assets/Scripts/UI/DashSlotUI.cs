using UnityEngine;
using UnityEngine.UI;

public class DashSlotUI : MonoBehaviour
{
    [SerializeField] private PlayerDash dash;
    [SerializeField] private Image slot;
    [SerializeField] private Image icon;

    [Header("Colours")]
    [SerializeField] private Color activeColor   = new Color32(158,0,93,255);
    [SerializeField] private Color inactiveColor = new Color32(50,50,50,255);

    private void Awake()
    {
        if (slot == null) slot = GetComponent<Image>();
        if (icon == null) Debug.LogError("No icon assigned");
        if (dash == null) dash = FindAnyObjectByType<PlayerDash>();
    }

    private void OnEnable()
    {
        if (dash != null) dash.OnDashAbilityChanged += HandleChanged;
        Refresh(dash != null && dash.HasDash);
    }

    private void OnDisable()
    {
        if (dash != null) dash.OnDashAbilityChanged -= HandleChanged;
    }

    private void HandleChanged(bool has) => Refresh(has);

    private void Refresh(bool has)
    {
        if (slot != null) slot.color = has ? activeColor : inactiveColor;
        if (icon != null) icon.color = has ? activeColor : inactiveColor;
    }
}