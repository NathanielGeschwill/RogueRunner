using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFollow : MonoBehaviour
{
    public GameObject player;
    public GameObject panel;
    public float j;
    public float xOff;

    private string zero;
    private string one;
    private string two;
    public TextMeshProUGUI platLayer;


   

    // Start is called before the first frame update
    void Start()
    {
        zero = "LAVA LAYER";
        one = "GRASS LAYER";
        two = "CLOUD LAYER";

        //LayerText(0);
        //PanalSwitch();
    }

    public void PanalSwitch()
    {
        //print("PANEL ACTIVE " + panel.active);
        if (panel.active)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }
        
    }

    public void LayerText(int Lel)
    {
        switch (Lel)
        {
            case 0:
                platLayer.text = zero;
                break;
            case 1:
                platLayer.text = one;
                break;
            case 2:
                platLayer.text = two;
                break;
        }
        PanalSwitch();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 target;
        target = new Vector3(player.transform.position.x + xOff, player.transform.position.y + 5f, transform.position.z);


        transform.position = Vector3.Lerp(transform.position, target, j);
    }
}
