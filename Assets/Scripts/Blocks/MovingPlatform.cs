using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 _lastPosition;
    public Vector3 DeltaPosition { get; private set; }

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        // ¬ычисл€ем, насколько блок сдвинулс€ за этот кадр
        DeltaPosition = transform.position - _lastPosition;
        _lastPosition = transform.position;
    }
}