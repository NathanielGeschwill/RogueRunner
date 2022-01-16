using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawn : MonoBehaviour
{
    //public GameManager gm;
    public Queue<GameObject> queuePlatforms;

    private float spawnScalar =20f;

    private int y;
    private int k;

    private float chanceToSpawn = .5f;

    public bool canSpawn = true;

    private IEnumerator cor;

    /*/
        TO DO: spawn in regions (Hell, Earth, sky?)
            more platform variety
            make sure platforms don't spawn in each other
                
            spawn hero platform? (player doesn't have to move while leveling up)
                
    //*/
    void Start()
    {
        //gm = FindObjectOfType<GameManager>();
        //SpawnPlatform(0);
        //queuePlatforms = new Queue<GameObject>();
        //GameManager.Instance.ws = this;
        StartCoroutine(SpawnChunk());
    }

    // Update is called once per frame
    void Update()
    {
        //float i = Random.Range(0, 100);
        //i += Mathf.Round(GameManager.Instance.speedDiff*spawnScalar*Time.deltaTime);
        
        //if (i > 99)
        //{
        //    //Debug.Log(i);
        //    SpawnPlatform(i);
        //}
    }

    public void SpawnPlatform(float yLevel)
    {
        //y = Random.Range(0, GameManager.Instance.platformSpawnY.Length);
        k = Random.Range(0, GameManager.Instance.platforms.Length);
        var ass = GameObject.Instantiate(GameManager.Instance.platforms[k], new Vector3(100, yLevel, 0f), transform.rotation);
        ass.transform.parent = gameObject.transform;
    }

    IEnumerator SpawnChunk()
    {
        while (canSpawn)
        {
            if(GameManager.Instance != null)
            {
                foreach (float f in GameManager.Instance.platformSpawnY)
                {
                    float chance = Random.Range(0.0f, 1.0f);
                    if (chance < chanceToSpawn)
                    {
                        SpawnPlatform(f);
                    }
                }
            }
            yield return new WaitForSeconds(spawnScalar / GameManager.Instance.worldSpeed);
        }
        
    }

    public Vector3 GetNewLocation()
    {
        y = Random.Range(0, GameManager.Instance.platformSpawnY.Length);
        k = Random.Range(0, GameManager.Instance.platforms.Length);
        return new Vector3(100, GameManager.Instance.platformSpawnY[y], 0f);
    }
}
