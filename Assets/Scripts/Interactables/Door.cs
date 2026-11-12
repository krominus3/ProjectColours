using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Vector3 moveDirection;
    [SerializeField] float moveTime = 0.5f;
    [SerializeField] Button button;

    private bool isMoving;
    private float moveSpeed;
    private Vector3 originalPosition;
    private Vector3 endPosition;
    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake() {
        originalPosition = transform.position;
        endPosition = originalPosition+moveDirection;
        moveSpeed = moveTime*10;
    }

    private void OnEnable() {
        button.perform += PerformAction;
    }

    private void OnDisable()
    {
        button.perform -= PerformAction;
    }

    private void Update() {
        if (isMoving)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                moveTime,
                moveSpeed,
                Time.deltaTime
            );
            if (Vector3.Distance(transform.position,targetPosition) < 0.005f)
            {
                isMoving = false;
                transform.position = targetPosition;
            }
        }
    }

    private void PerformAction(bool var)
    {
        targetPosition = var ? endPosition : originalPosition;
        isMoving = true;
    }
}
