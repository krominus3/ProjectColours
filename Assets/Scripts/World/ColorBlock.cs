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

        // ���������� ������
        _rend.GetPropertyBlock(_mpb);
        _rend.enabled = !sameColor;
        //_mpb.SetFloat("_Alpha", sameColor ? 0.25f : 1f);//Chtoby rabotalo luchshe sozdat' otdel'nye materialy s "Surface Type = Transparent". "Opaque" daet teni no ne daet prozrachnost' "Transparent" naobarot
        _rend.SetPropertyBlock(_mpb);
    }
}