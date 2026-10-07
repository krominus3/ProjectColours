using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class LensInventory : MonoBehaviour
{
    [SerializeField] private LensMixTable mixTable;

    private readonly List<LensData> _equipped = new();
    public event Action<LensType> OnLensChanged;

    public LensType CurrentColor { get; private set; } = LensType.None;

    private void Awake()
    {
        if (mixTable == null)
        {
            Debug.LogError("Не подключена таблица смешивания!");
        }
    }

    public void Equip(LensData lens)
    {
        if (_equipped.Contains(lens)) return;
        _equipped.Add(lens);
        Recalculate();
    }

    public void Unequip(LensData lens)
    {
        if (_equipped.Remove(lens)) Recalculate();
    }

    public bool IsEquipped(LensData l) => _equipped.Contains(l);

    public void ClearAll()
    {
        _equipped.Clear();
        Recalculate();
    }

    private void Recalculate()
    {
        var types = new List<LensType>();
        foreach (var l in _equipped) types.Add(l.type);

        CurrentColor = mixTable.MixAll(types);
        OnLensChanged?.Invoke(CurrentColor);
    }
}