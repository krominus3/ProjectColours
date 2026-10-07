using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColorBlock : MonoBehaviour
{
    public LensType blockColor = LensType.Red;

    private Collider _col;
    private Renderer _rend;
    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _rend = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    public void ReactToLens(LensType current)
    {
        bool sameColor = (current == blockColor);
        _col.enabled = !sameColor;

        // Визуальный отклик
        _rend.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Alpha", sameColor ? 0.25f : 1f);
        _rend.SetPropertyBlock(_mpb);
    }
}