using UnityEngine;

public class BlockEffectController : MonoBehaviour
{
    [Header("Ёффекты этого блока")]
    public bool hasDoubleJump = true;
    public bool hasDash = true;
    public bool hasMagnet = true;

    private ColoredBlock _block;
    private GameObject _player;
    private bool _dashGiven;

    // —сылка на т€желый скрипт магнита
    private CyanMagnetBlock _magnetBlock;
    private YellowMovingBlock _yellowBlock;

    private void Awake()
    {
        _block = GetComponentInParent<ColoredBlock>();
        _magnetBlock = GetComponent<CyanMagnetBlock>();

        // ѕри старте выключаем магнит
        if (_magnetBlock != null) _magnetBlock.enabled = false;

        _yellowBlock = GetComponentInChildren<YellowMovingBlock>();
        if (_yellowBlock != null) _yellowBlock.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _player = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != _player) return;
        SetDash(false);
        _player = null;
    }

    private void Update()
    {
        if (_player != null)
        {
            SetDash(hasDash && _block.currentColor == LensType.Purple);
        }

        // ”правл€ем магнитом (включаем/выключаем скрипт)
        if (_magnetBlock != null)
        {
            bool isCyan = _block.currentColor == LensType.Cyan && hasMagnet;
            if (_magnetBlock.enabled != isCyan)
            {
                _magnetBlock.enabled = isCyan;
            }
        }

        if (_yellowBlock != null)
        {
            bool isYellow = _block.currentColor == LensType.Yellow;
            if (_yellowBlock.enabled != isYellow)
            {
                _yellowBlock.enabled = isYellow;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != _player) return;

        switch (_block.currentColor)
        {
            case LensType.Green when hasDoubleJump:
                if (_player.TryGetComponent(out PlayerMovement movement))
                    movement.AddExtraJump();
                break;
        }
    }

    private void SetDash(bool give)
    {
        if (give == _dashGiven) return;
        _dashGiven = give;

        if (_player != null && _player.TryGetComponent(out PlayerDash dash))
        {
            if (give) dash.AddDash();
            else dash.RemoveDash();
        }
    }
}