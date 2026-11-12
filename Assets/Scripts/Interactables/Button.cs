using System;
using UnityEngine;

public class Button : MonoBehaviour
{
    public event Action<bool> perform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Block"))
        {
            perform(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Block"))
        {
            perform(false);
        }
    }
}
