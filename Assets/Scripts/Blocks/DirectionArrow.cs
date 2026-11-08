using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    [Tooltip("Радиус зоны вокруг центра стрелки, куда нужно смотреть, чтобы ее повернуть")]
    [SerializeField] private float arrowHitRadius = 0.5f;

    // Метод, который будет вызывать игрок при нажатии E
    public bool TryRotate(Vector3 hitPoint)
    {
        // Проверяем, попал ли игрок в центр стрелки
        if (Vector3.Distance(hitPoint, transform.position) <= arrowHitRadius)
        {
            // Крутим на 90 градусов по часовой (вокруг локальной оси Y)
            transform.Rotate(0, 90f, 0, Space.Self);
            return true; // Успешно повернули
        }
        return false; // Игрок нажал мимо
    }

    // Удобный геттер для направления
    public Vector3 ForwardDirection => transform.forward;
}