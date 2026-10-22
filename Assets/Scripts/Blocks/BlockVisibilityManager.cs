using UnityEngine;

public class BlockVisibilityManager : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LensInventory inventory;

    [Header("Настройки производительности")]
    [Tooltip("Как часто проверять видимость (в секундах). 0.1 = 10 раз в секунду.")]
    [SerializeField] private float updateInterval = 0.1f;

    private float _timer;
    private Plane[] _frustumPlanes = new Plane[6];

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (inventory == null) inventory = FindAnyObjectByType<LensInventory>();
    }

    private void Update()
    {
        if (playerCamera == null || inventory == null) return;

        // Оптимизация: проверяем не каждый кадр, а с заданным интервалом
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer = updateInterval;

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        // Получаем 6 плоскостей поля зрения камеры
        GeometryUtility.CalculateFrustumPlanes(playerCamera, _frustumPlanes);

        LensType currentLens = inventory.CurrentColor;

        // Проходим по всем блокам в реестре
        foreach (var block in ColoredBlock.AllBlocks)
        {
            if (block == null) continue;

            // Проверяем, пересекается ли блок с полем зрения
            bool isVisible = GeometryUtility.TestPlanesAABB(_frustumPlanes, block.GetBounds());

            if (isVisible)
            {
                // Если в поле зрения — применяем эффект
                block.ReactToLens(currentLens);
            }
            else
            {
                // Если за кадром — сбрасываем
                block.ResetToBaseState();
            }
        }
    }
}