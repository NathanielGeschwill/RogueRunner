using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameManager gm;
    public GameObject player;
    //public Camera gc;
    
    public Vector3 offset;
    public Vector3 shakeOffset;
    public Vector3 speedandairOffset;

    public float smoothSpeed = .125f;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        //gc = FindObjectOfType<Camera>();
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        offset = transform.position - player.transform.position;
        shakeOffset = new Vector3(0, 0, 0);
        speedandairOffset = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float tempSpeed = gm.speedDiff; // -6
        float tempAir = gm.airTime; // -35
        Mathf.Clamp(tempSpeed, 0, 6);
        Mathf.Clamp(tempAir, 0, 35);

        //Debug.Log(gm.speedDiff + " " + gm.airTime);

        speedandairOffset = new Vector3(-tempSpeed, 0, -tempAir);
        
    }



    private void FixedUpdate()
    {
        Vector3 desiredPos = player.transform.position + offset + speedandairOffset;
        //desiredPos = new Vector3( desiredPos.x, desiredPos.y , Mathf.Clamp( desiredPos.z, player.transform.position.z + offset.z, -40 ) );
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed );

        
        //Debug.Log("Desired " + desiredPos + "    smoothed: " + smoothedPos);

        transform.position = smoothedPos;

    }
}
