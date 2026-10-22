using UnityEngine;
using UnityEngine.UI;

public abstract class BlockBehavior : MonoBehaviour
{
    // Какой цвет должен быть у блока, чтобы это поведение включилось
    public abstract LensType RequiredColor { get; }

    protected ColoredBlock Block { get; private set; }

    protected virtual void Awake()
    {
        Block = GetComponent<ColoredBlock>();
    }

    // Вызывается, когда цвет блока меняется
    public virtual void OnColorChanged(LensType currentColor)
    {
        // Включаем/выключаем компонент в зависимости от цвета
        enabled = (currentColor == RequiredColor);
    }
}