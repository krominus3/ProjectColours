using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColoredBlock : MonoBehaviour
{
    // Статический реестр всех активных блоков в сцене
    public static readonly List<ColoredBlock> AllBlocks = new List<ColoredBlock>();

    [Header("Цвета")]
    public LensType baseColor = LensType.Red;

    [HideInInspector]
    public LensType currentColor = LensType.Red;

    [Header("Настройки")]
    [Tooltip("Если включено, блок исчезает при совпадении цвета линзы с базовым цветом блока.")]
    public bool makePassable = true;

    [Header("Ссылки")]
    [SerializeField] private LensMixTable mixTable;

    private Collider _col;
    private Renderer _rend;
    private BlockBehavior[] _behaviors;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _rend = GetComponent<Renderer>();
        _behaviors = GetComponents<BlockBehavior>();

        if (mixTable == null) mixTable = FindAnyObjectByType<LensMixTable>();
    }

    private void OnEnable()
    {
        // Регистрируемся в реестре при появлении на сцене
        if (!AllBlocks.Contains(this))
            AllBlocks.Add(this);
    }

    private void OnDisable()
    {
        // Удаляемся из реестра при выключении/удалении
        AllBlocks.Remove(this);
    }

    private void Start()
    {
        ApplyColor(baseColor, LensType.None);
    }

    public void ReactToLens(LensType lensColor)
    {
        // Вычисляем новый цвет (смешиваем базовый и линзу)
        LensType newColor = mixTable != null ? mixTable.Mix(baseColor, lensColor) : baseColor;
        ApplyColor(newColor, lensColor);
    }

    private void ApplyColor(LensType newColor, LensType lensColor)
    {
        currentColor = newColor;

        // Логика исчезновения
        bool isPassable = makePassable && (lensColor == baseColor) && (lensColor != LensType.None);

        if (_col != null) _col.enabled = !isPassable;
        if (_rend != null) _rend.enabled = !isPassable;

        // Сообщаем поведениям (для будущих механик)
        if (_behaviors != null)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnColorChanged(currentColor);
            }
        }
    }

    // Метод для сброса в базовое состояние (когда блок вне поля зрения)
    public void ResetToBaseState()
    {
        ApplyColor(baseColor, LensType.None);
    }

    // Получаем габариты блока для проверки видимости
    public Bounds GetBounds()
    {
        if (_rend != null) return _rend.bounds;
        if (_col != null) return _col.bounds;
        return new Bounds(transform.position, Vector3.one);
    }
}