using UnityEngine;
using UnityEngine.UI;

public class LensSlotUI : MonoBehaviour
{
    [SerializeField] private LensData lens;
    [SerializeField] private LensInventory inventory;
    [SerializeField] private Image slotImage;

    [Header("Colours")]
    [SerializeField] private Color activeColor   = new Color32(251,176,59,255);
    [SerializeField] private Color inactiveColor = new Color32(50,50,50,255);

    private void Awake()
    {
        if (slotImage == null) slotImage = GetComponent<Image>();
        if (inventory == null) inventory = FindAnyObjectByType<LensInventory>();
    }

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnLensChanged += HandleLensChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnLensChanged -= HandleLensChanged;
    }

    private void HandleLensChanged(LensType _) => Refresh();

    private void Refresh()
    {
        if (slotImage == null || lens == null || inventory == null) return;

        bool equipped = inventory.IsEquipped(lens);
        slotImage.color = equipped ? activeColor : inactiveColor;
    }
}