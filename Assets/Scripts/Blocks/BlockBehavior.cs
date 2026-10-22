using UnityEngine;

public abstract class BlockBehavior : MonoBehaviour
{
    public abstract LensType RequiredColor { get; }

    protected ColoredBlock Block { get; private set; }
    protected bool IsActive { get; private set; }

    protected virtual void Awake()
    {
        // »щем ColoredBlock на себе, а если нет Ч на родителе
        Block = GetComponent<ColoredBlock>();
        if (Block == null) Block = GetComponentInParent<ColoredBlock>();

        IsActive = false;
    }

    public virtual void OnColorChanged(LensType currentColor)
    {
        IsActive = (currentColor == RequiredColor);
    }
}