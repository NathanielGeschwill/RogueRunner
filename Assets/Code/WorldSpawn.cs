using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawn : MonoBehaviour
{
    public GameManager gm;
    public Queue<GameObject> queuePlatforms;

    private int y;
    private int k;

    /*/
        TO DO: spawn in regions (Hell, Earth, sky?)
            more platform variety
            make sure platforms don't spawn in each other
                
            spawn hero platform? (player doesn't have to move while leveling up)
                
    //*/
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        spawnPlatform(0);
        queuePlatforms = new Queue<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        float i = Random.Range(0, 100);
        i += Mathf.Round(gm.speedDiff*2);
        
        if (i > 99)
        {
            //Debug.Log(i);
            spawnPlatform(i);
        }
    }

    public void spawnPlatform(float lel)
    {

        

        y = Random.Range(0, gm.platformSpawnY.Length);
        k = Random.Range(0, gm.platforms.Length);
        var ass = GameObject.Instantiate(gm.platforms[k], new Vector3(100, gm.platformSpawnY[y], 0f), transform.rotation);
        ass.transform.parent = gameObject.transform;
    }
}
