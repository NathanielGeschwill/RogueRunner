using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScroll : MonoBehaviour {


    public GameManager gm;
    private float worldSpeed;
        

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        //spawned = true;
        worldSpeed = gm.worldSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(worldSpeed != gm.worldSpeed)
        {
            worldSpeed = gm.worldSpeed;
        }
    }

    void FixedUpdate()
    {
        //if (spawned) { spawned = false; }

        Vector3 pos = transform.position;

        pos.x -= worldSpeed * Time.deltaTime;

        transform.position = pos;


        if(transform.position.x <= -80)
        {
            gm.DestorySelf(this.gameObject); //queue up
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Platform"))
        {
            
            gm.DestorySelf(other.gameObject); // queue up
            
        }
    }

}
