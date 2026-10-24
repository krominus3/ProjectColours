using UnityEngine;

public class BlueSizeBlock : MonoBehaviour
{
    [Header("Настройки размера")]
    [SerializeField] private Vector3 maxScale = new Vector3(2f, 2f, 2f);
    [SerializeField] private float growSpeed = 2f;
    [SerializeField] private LayerMask obstacleMask = ~0;

    private ColoredBlock _block;
    private Transform _rootObject;

    private Vector3 _initialScale;
    private Vector3 _initialPosition;

    private void Awake()
    {
        _block = GetComponentInParent<ColoredBlock>();
        _rootObject = _block != null ? _block.transform : transform;

        _initialScale = _rootObject.localScale;
        _initialPosition = _rootObject.position;
    }

    private void Update()
    {
        bool isBlue = _block != null && _block.currentColor == LensType.Blue;

        Vector3 currentScale = _rootObject.localScale;
        Vector3 targetScale = isBlue ? maxScale : _initialScale;

        // Если мы уже достигли нужного размера — ничего не делаем
        if (Vector3.Distance(currentScale, targetScale) < 0.01f) return;

        Vector3 newScale = currentScale;

        // Проверяем рост/сжатие по каждой оси отдельно
        for (int i = 0; i < 3; i++)
        {
            float targetAxis = targetScale[i];
            float currentAxis = currentScale[i];
            float delta = targetAxis - currentAxis;

            // Если мы растём
            if (delta > 0)
            {
                float step = growSpeed * Time.deltaTime;
                step = Mathf.Min(step, delta); // Не перепрыгиваем цель

                // Проверяем, есть ли место для роста в эту сторону
                if (CanGrow(currentAxis, step, i))
                {
                    newScale[i] += step;
                }
            }
            // Если мы сжимаемся
            else if (delta < 0)
            {
                float step = growSpeed * Time.deltaTime;
                step = Mathf.Min(step, -delta);
                newScale[i] -= step; // Сжимаемся без проверки стен
            }
        }

        // Применяем новый масштаб
        _rootObject.localScale = newScale;

        // Сдвигаем блок вверх, чтобы он рос от пола (а не в пол)
        float heightDiff = _rootObject.localScale.y - _initialScale.y;
        _rootObject.position = _initialPosition + new Vector3(0, heightDiff / 2f, 0);
    }

    private bool CanGrow(float currentSize, float step, int axisIndex)
    {
        // Вычисляем размер после роста
        Vector3 testSize = _rootObject.localScale;
        testSize[axisIndex] += step;

        // Половина размера (радиус)
        Vector3 halfExtents = testSize / 2f;
        // Чуть-чуть уменьшаем, чтобы не цеплять пол
        halfExtents *= 0.95f;

        Vector3 direction = Vector3.zero;
        direction[axisIndex] = 1f; // Направление роста по оси

        // Пускаем виртуальный "куб" в сторону роста
        // Если он сталкивается с чем-то (кроме самого себя) — места нет
        bool hasHit = Physics.BoxCast(_rootObject.position, halfExtents, direction,
                                      out RaycastHit hit, _rootObject.rotation, 0.01f, obstacleMask);

        return !hasHit;
    }
}