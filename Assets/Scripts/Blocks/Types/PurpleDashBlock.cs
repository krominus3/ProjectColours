using UnityEngine;

public class PurpleDashBlock : BlockBehavior
{
    public override LensType RequiredColor => LensType.Purple;

    private void OnTriggerStay(Collider other)
    {
        if (!enabled) return;

        if (other.CompareTag("Player"))
        {
            var playerDash = other.GetComponent<PlayerDash>();
            if (playerDash != null)
            {
                playerDash.AddDash();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled) return;

        if (other.CompareTag("Player"))
        {
            var playerDash = other.GetComponent<PlayerDash>();
            if (playerDash != null)
            {
                playerDash.RemoveDash();
            }
        }
    }
}