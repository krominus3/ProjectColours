using UnityEngine;

public class GreenDoubleJumpBlock : BlockBehavior
{
    public override LensType RequiredColor => LensType.Green;

    // OnTriggerStay срабатывает каждый кадр, пока игрок внутри триггера
    private void OnTriggerStay(Collider other)
    {
        if (!enabled) return;

        if (other.CompareTag("Player"))
        {
            var playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.AddExtraJump();
                Debug.Log("[GreenBlock] Игроку выдан двойной прыжок!");
            }
        }
    }
}