using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerTrigger : MonoBehaviour
{

    public delegate void KillPlayer();
    public static event KillPlayer OnKillPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnKillPlayer?.Invoke();
        }
    }
}
