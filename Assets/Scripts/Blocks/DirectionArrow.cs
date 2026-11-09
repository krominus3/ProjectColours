using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Включена ли стрелка. Если выключить — она исчезнет, а блок станет обычным ящиком.")]
    public bool isActive = true;

    [SerializeField] private float arrowHitRadius = 0.5f;

    // Ссылка на визуал (чтобы скрывать только меш, а не весь объект)
    [SerializeField] private GameObject visualObject;

    private void Start()
    {
        UpdateVisibility();
    }

    // Метод для включения/выключения
    public void SetActive(bool state)
    {
        isActive = state;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (visualObject != null)
            visualObject.SetActive(isActive);
        else
            gameObject.SetActive(isActive); // Если визуал не указан, выключаем весь объект
    }

    public bool TryRotate(Vector3 hitPoint)
    {
        if (!isActive) return false; // Если выключена, не крутим

        if (Vector3.Distance(hitPoint, transform.position) <= arrowHitRadius)
        {
            transform.Rotate(0, 90f, 0, Space.Self);
            return true;
        }
        return false;
    }

    public Vector3 ForwardDirection => transform.forward;
}