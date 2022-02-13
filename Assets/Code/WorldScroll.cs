using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScroll : MonoBehaviour {


    public GameManager gm;
    private float worldSpeed;
    [SerializeField]
    private float paralax = 1;

    public bool tp = false;
        

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
        Vector3 pos = transform.position;
        
        pos.x -= (worldSpeed * (1/paralax))* Time.deltaTime;

        transform.position = pos;


        if(transform.position.x <= -80)
        {
            if (tp)
            {
                transform.position = new Vector3(958, transform.position.y, transform.position.z);
            }
            else
            {
                //print("die of x");
                gm.DestorySelf(this.gameObject); //queue up
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Platform"))
        {
            //print("die of trig");
            //gm.DestorySelf(other.gameObject); // queue up
            other.gameObject.transform.position = GameManager.Instance.ws.GetNewLocation();
            
        }
    }

}
