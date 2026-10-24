using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColoredBlock : MonoBehaviour
{
    public static readonly List<ColoredBlock> AllBlocks = new List<ColoredBlock>();

    [Header("Цвета")]
    public LensType baseColor = LensType.Red;
    [HideInInspector] public LensType currentColor = LensType.Red;

    [Header("Настройки")]
    public bool makePassable = true;

    [SerializeField] private LensMixTable mixTable;

    private Collider _col;
    private Renderer _rend;
    private GameObject _child;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _rend = GetComponent<Renderer>();
        _child = transform.GetChild(0).gameObject;
        if (mixTable == null) mixTable = FindAnyObjectByType<LensMixTable>();
    }

    private void OnEnable() => AllBlocks.Add(this);
    private void OnDisable() => AllBlocks.Remove(this);

    private void Start()
    {
        ApplyColor(baseColor, LensType.None);
    }

    public void ReactToLens(LensType lensColor)
    {
        LensType newColor = mixTable != null ? mixTable.Mix(baseColor, lensColor) : baseColor;
        ApplyColor(newColor, lensColor);
    }

    private void ApplyColor(LensType newColor, LensType lensColor)
    {
        currentColor = newColor;

        bool isPassable = makePassable && (lensColor == baseColor) && (lensColor != LensType.None);
        //gameObject.SetActive(!isPassable);
        if (_col != null) _col.enabled = !isPassable;
        if (_rend != null) _rend.enabled = !isPassable;
        if (_child != null)
        {
            var childCol = _child.GetComponent<Collider>();
            if (childCol != null) childCol.enabled = !isPassable;
            var childRend = _child.GetComponent<Renderer>();
            if (childRend != null) childRend.enabled = !isPassable;
        }

        // Находим все скрипты поведения на блоке и на его дочерних объектах (для триггеров)
        var behaviors = GetComponentsInChildren<BlockBehavior>(true);
        foreach (var behavior in behaviors)
        {
            behavior.OnColorChanged(currentColor);
        }
    }

    public Bounds GetBounds() => _rend != null ? _rend.bounds : new Bounds(transform.position, Vector3.one);
    public void ResetToBaseState() => ApplyColor(baseColor, LensType.None);
}