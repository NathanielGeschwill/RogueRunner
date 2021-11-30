using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{

    public GameObject player; //da playa (world moves around player, player only moves on y for time being)
    public float worldSpeed = 15; //world speed that changes based on the situation
    public float targetWorldSpeed = 15; // world speed that is always returned to
    public bool playerSpeeding = false;
    public bool stopped = false;

    public float speedDiff; //used in calculations for camera offset
    public float airTime; //used in camera offset + achievements?highscore?


    public GameObject[] platforms;
    public float[] platformsSkyY; 
    public float[] platformSpawnY;
    public float[] platformsHellY;


    private void Start()
    {
        
    }

    private void Update()
    {
        //return worldspeed to the target world speed
        float x = Mathf.Lerp(worldSpeed, targetWorldSpeed, .004f);
        
        
        worldSpeed = x;

        if (worldSpeed != 0)
        {
            speedDiff = worldSpeed / targetWorldSpeed;
        }
        else
        {
            speedDiff = 0;
        }
    }

    //all changes to worldspeed use this function
    public void worldSpeedChange(bool multi, float val)
    {
        if (!stopped)
        {

        
            if (multi)
            {
                worldSpeed *= val;
            }
            else
            {
                worldSpeed += val;
            }
        }
    }

    //rip prefab and kids
    public void DestorySelf(GameObject g)
    {
        foreach(Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(g);
    }


    [ContextMenu(  "WorldSpeedZero()" )]
    void WorldSpeedZero()
    {
        targetWorldSpeed = 0;
        worldSpeed = 0;
    }
    
}
