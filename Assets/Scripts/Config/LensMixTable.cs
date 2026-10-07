using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lens/MixTable")]
public class LensMixTable : ScriptableObject
{
    [Serializable]
    public struct MixRule
    {
        public LensType a;
        public LensType b;
        public LensType result;
    }

    public List<MixRule> rules = new();

    // Кэш для быстрого поиска
    private Dictionary<(LensType, LensType), LensType> _cache;

    private void BuildCache()
    {
        _cache = new Dictionary<(LensType, LensType), LensType>();
        foreach (var r in rules)
        {
            _cache[(r.a, r.b)] = r.result;
            _cache[(r.b, r.a)] = r.result; // коммутативность
        }
    }

    public LensType Mix(LensType a, LensType b)
    {
        if (_cache == null) BuildCache();
        if (a == LensType.None) return b;
        if (b == LensType.None) return a;
        if (a == b) return a;

        return _cache.TryGetValue((a, b), out var r) ? r : LensType.Black;
    }

    // Смешать список всех надетых линз
    public LensType MixAll(IEnumerable<LensType> lenses)
    {
        LensType acc = LensType.None;
        foreach (var l in lenses)
            acc = Mix(acc, l);
        return acc;
    }
}