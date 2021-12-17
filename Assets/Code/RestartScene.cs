using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{

    private void OnEnable()
    {
        IEntity.OnDeath += Reload;
        KillPlayerTrigger.OnKillPlayer += Reload;
    }

    private void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Reload(object sender, int senderID)
    {
        if (((GameObject)sender).CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
