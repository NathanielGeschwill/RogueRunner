using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInterest : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;
    public GameObject camMenu;
    private Transform start;
    private Vector3 n;
    private Quaternion o;
    private bool menu = false;

    public GameObject hsUI;
    public GameObject menuUI;

    public float smoothPos;
    public float smoothRot;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        start = transform;

        smoothPos = .09f;
        smoothRot = .2f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition));
        Vector3 m = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (menu)
        {
            n = camMenu.transform.position;
        }
        else
        {
            n = Vector3.Lerp(cam1.transform.position, cam2.transform.position, m.y);
            o = Quaternion.Lerp(cam1.transform.rotation, cam2.transform.rotation, m.x);
        }
        


    }

    private void FixedUpdate()
    {
        PosUpdate();
        RotUpdate();
    }

    private void PosUpdate() => transform.position = Vector3.Lerp(transform.position, n, smoothPos);

    private void RotUpdate() => transform.rotation = Quaternion.Lerp(transform.rotation, o, smoothRot);

    public void hsButton()
    {
        menuUI.SetActive(false);
        hsUI.SetActive(true);
        menu = true;
    }

    public void menuBack()
    {
        menuUI.SetActive(true);
        hsUI.SetActive(false);
        menu = false;
    }
}
