using UnityEngine;

public class BlockVisibilityManager : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LensInventory inventory;
    [SerializeField] private Collider player;

    [Header("Настройки производительности")]
    [Tooltip("Как часто проверять видимость (в секундах). 0.1 = 10 раз в секунду.")]
    [SerializeField] private float updateInterval = 0.1f;

    private float _timer;
    private Plane[] _frustumPlanes = new Plane[6];

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (inventory == null) inventory = FindAnyObjectByType<LensInventory>();

        if (player == null)
        {
            var pc = FindAnyObjectByType<PlayerMovement>();
            if (pc != null) player = pc.transform.GetComponent<Collider>();
        }
    }

    private void Update()
    {
        if (playerCamera == null || inventory == null) return;

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer = updateInterval;

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        GeometryUtility.CalculateFrustumPlanes(playerCamera, _frustumPlanes);

        LensType currentLens = inventory.CurrentColor;

        foreach (var block in ColoredBlock.AllBlocks)
        {
            if (block == null) continue;

            // Если игрок внутри блока — не трогаем блок вообще,
            // чтобы он не "включился" обратно и не зажал игрока.
            if (inventory.CurrentColor == block.baseColor)
                if (IsPlayerInside(block))
                {
                    block.ReactToLens(currentLens);
                    continue;
                }

            bool isVisible = GeometryUtility.TestPlanesAABB(_frustumPlanes, block.GetBounds());

            if (isVisible)
                block.ReactToLens(currentLens);
            else
                block.ResetToBaseState();
        }
    }

    private bool IsPlayerInside(ColoredBlock block)
    {
        if (player == null) return false;

        Bounds blockBounds = block.GetBounds();
        Bounds playerBounds = player.bounds;
        playerBounds.Expand(-0.05f);

        return blockBounds.Intersects(playerBounds);
    }
}