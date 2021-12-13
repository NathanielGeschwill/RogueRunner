using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        IEntity.OnDeath += Reload;
    }

    private void Reload(object sender, int senderID)
    {
        if (((GameObject)sender).CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
