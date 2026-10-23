using UnityEngine;

public abstract class BlockBehavior : MonoBehaviour
{
    public abstract LensType RequiredColor { get; }

    protected ColoredBlock Block { get; private set; }

    protected virtual void Awake()
    {
        Block = GetComponent<ColoredBlock>();
        if (Block == null) Block = GetComponentInParent<ColoredBlock>();
        enabled = false; // Выключаем при старте
    }

    // Вызывается из ColoredBlock при смене цвета
    public virtual void OnColorChanged(LensType currentColor)
    {
        enabled = (currentColor == RequiredColor);
    }
}