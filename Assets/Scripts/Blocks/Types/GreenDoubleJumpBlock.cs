using UnityEngine;

public class GreenDoubleJumpBlock : BlockBehavior
{
    public override LensType RequiredColor => LensType.Green;

    private void OnTriggerStay(Collider other)
    {
        // Если поведение не активно (блок не зеленый) — ничего не делаем
        if (!IsActive) return;

        if (other.CompareTag("Player"))
        {
            var playerMovement = other.GetComponent<PlayerMovement>();
            //var playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.AddExtraJump(1);
                Debug.Log("[GreenBlock] Игроку выдан двойной прыжок!");
            }
        }
    }
}