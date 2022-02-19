using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{

    private void OnEnable()
    {
        IEntity.OnDeath += ShowLoseUI;
        KillPlayerTrigger.OnKillPlayer += ShowLoseUI;
    }

    public void Reload()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ShowLoseUI()
    {
        Time.timeScale = 0f;
        GameManager.Instance.gamePaused = true;
        GameManager.Instance.loseUI.SetActive(true);
        GameManager.Instance.gameUI.SetActive(false);
    }

    private void ShowLoseUI(object sender, int senderID)
    {
        if (((GameObject)sender).CompareTag("Player"))
        {
            GameManager.Instance.SetLoseText();
            //print("Before lose UI");
            ShowLoseUI();
            //print("after show lose UI");
            
        }
    }
}
