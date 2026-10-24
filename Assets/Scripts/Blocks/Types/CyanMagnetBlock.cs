using UnityEngine;

public class CyanMagnetBlock : MonoBehaviour
{
    [Header("Настройки магнита")]
    [SerializeField] private float magnetForce = 15f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private bool isAttracting = true;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Визуал")]
    [SerializeField] private LineRenderer lineRenderer;

    private void OnEnable() { if (lineRenderer) lineRenderer.enabled = true; }
    private void OnDisable() { if (lineRenderer) lineRenderer.enabled = false; }

    private void FixedUpdate()
    {
        Vector3 origin = transform.position;
        Vector3 end = origin + transform.forward * maxDistance;

        bool hasHit = Physics.Raycast(origin, transform.forward, out RaycastHit hit, maxDistance,
                            hitMask, QueryTriggerInteraction.Ignore);

        if (hasHit)
        {
            end = hit.point;
            if (hit.collider.CompareTag("Magnetic"))
                ApplyForce(hit);
        }

        Debug.DrawLine(origin, end, hasHit ? Color.red : Color.green);

        if (lineRenderer)
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, end);
        }
    }

    private void ApplyForce(RaycastHit hit)
    {
        Vector3 direction = (transform.position - hit.point).normalized;
        if (!isAttracting) direction = -direction;

        if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
        {
            hit.rigidbody.AddForce(direction * magnetForce, ForceMode.Force);
        }
        else if (hit.collider.TryGetComponent(out CharacterController controller))
        {
            controller.Move(direction * magnetForce * Time.fixedDeltaTime);
        }
    }
}