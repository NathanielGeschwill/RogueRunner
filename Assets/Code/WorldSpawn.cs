using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawn : MonoBehaviour
{
    public HorizontalPlatformSO[] hpsos;
    public HorizontalPlatformSO[] bossHPSOS;
    private List<HorizontalPlatformSpawner> hps = new List<HorizontalPlatformSpawner>();
    public Material[] materials;
    private float hellLevel = 0.0f;
    private float skyLevel = 60.0f;

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
        
        foreach(HorizontalPlatformSO h in hpsos)
        {
            hps.Add(gameObject.AddComponent<HorizontalPlatformSpawner>());

            if(h.yLevel >= skyLevel)
            {
                hps[hps.Count - 1].SetWorldSpawn(this, h, bossHPSOS[(hps.Count - 1) % 3], materials[2], hps.Count - 1);
            }
            else if(h.yLevel <= hellLevel)
            {
                hps[hps.Count - 1].SetWorldSpawn(this, h, bossHPSOS[(hps.Count - 1) % 3], materials[0], hps.Count - 1);
            }
            else
            {
                hps[hps.Count - 1].SetWorldSpawn(this, h, bossHPSOS[(hps.Count - 1) % 3], materials[1], hps.Count - 1);
            }
            //print(bossHPSOS[(hps.Count - 1) % 3]);
        }
    }

    public void SpawnBoss(int id)
    {
        hps[id].SpawnBoss();
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
