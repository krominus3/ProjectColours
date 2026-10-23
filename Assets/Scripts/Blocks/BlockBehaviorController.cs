using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColoredBlock))]
public class BlockBehaviorController : MonoBehaviour
{
    private ColoredBlock _block;
    private List<BlockBehavior> _behaviors = new List<BlockBehavior>();

    private void Awake()
    {
        _block = GetComponent<ColoredBlock>();

        // Собираем ТОЛЬКО те скрипты поведения, которые есть на блоке
        _behaviors.AddRange(GetComponentsInChildren<BlockBehavior>());

        // При старте выключаем ВСЕ поведения (чтобы они не работали до смены цвета)
        foreach (var behavior in _behaviors)
        {
            behavior.enabled = false;
        }
    }

    // Этот метод будет вызывать ColoredBlock при смене цвета
    public void OnColorChanged(LensType newColor)
    {
        foreach (var behavior in _behaviors)
        {
            // Включаем скрипт, только если его требуемый цвет совпадает с текущим
            behavior.enabled = (behavior.RequiredColor == newColor);
        }
    }
}