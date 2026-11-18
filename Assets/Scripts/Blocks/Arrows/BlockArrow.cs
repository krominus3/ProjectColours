using System;
using UnityEngine;

/// <summary>
/// Основа всех стрелок на блоках: состояние, видимость и взаимодействие.
/// Зона взаимодействия — коллайдер самой стрелки: куда игрок целится, ту стрелку и нажимает.
/// Коллайдер масштабируется вместе с блоком, поэтому стрелки работают при любом размере блока.
/// Конкретное поведение задают наследники (OnInteract) и владелец блока (событие Interacted).
/// </summary>
public abstract class BlockArrow : MonoBehaviour
{
    [Header("Состояние")]
    [Tooltip("Активна ли стрелка. Выключенная стрелка скрыта и не реагирует на взаимодействие.")]
    public bool isActive = true;

    [Header("Визуал")]
    [Tooltip("Модель стрелки. Если не задана, при выключении стрелки отключается весь её объект.")]
    [SerializeField] protected GameObject visualObject;

    /// <summary>Вызывается после того, как игрок нажал на стрелку.</summary>
    public event Action<BlockArrow> Interacted;

    /// <summary>
    /// true — стрелка реагирует и на взаимодействие с любой частью блока, а не только на прицеливание в неё.
    /// </summary>
    public virtual bool AcceptsAnyPartOfBlock => false;

    protected virtual void Start()
    {
        UpdateVisibility();

        if (!AcceptsAnyPartOfBlock && GetComponentInChildren<Collider>(true) == null)
            Debug.LogWarning($"[{name}] У стрелки нет коллайдера: в неё нельзя прицелиться. " +
                             "Добавь BoxCollider (Is Trigger), чуть выступающий над гранью блока.", this);
    }

    /// <summary>Включить или выключить стрелку (например, из переключателя).</summary>
    public void SetActive(bool state)
    {
        isActive = state;
        UpdateVisibility();
    }

    protected void UpdateVisibility()
    {
        if (visualObject != null)
            visualObject.SetActive(isActive);
        else
            gameObject.SetActive(isActive);
    }

    /// <summary>Нажатие игрока на стрелку.</summary>
    public void Interact()
    {
        if (!isActive) return;
        OnInteract();
        Interacted?.Invoke(this);
    }

    protected virtual void OnInteract() { }
}