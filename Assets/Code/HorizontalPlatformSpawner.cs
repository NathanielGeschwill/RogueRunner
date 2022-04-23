using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HorizontalPlatformSpawner : MonoBehaviour
{
    private HorizontalPlatformSO hpso;
    private HorizontalPlatformSO bossHPSO;
    private WorldSpawn ws;
    public bool canSpawn = true;
    //private float worldScaleConst = 15.0f;
    //private float worldScaleMod = 10.0f;
    //private float timer = 1000;
    private float nextDistanceToSpawn;
    private float distanceTraveled;
    private bool readyToSpawn = false;
    private Material material;
    float platLength = 16f;
    float DEFAULT_PLAT_LENGTH = 16f;
    int levelId = -1;
    private bool spawnBoss = false;

    public void SetWorldSpawn(WorldSpawn ws, HorizontalPlatformSO hpso, HorizontalPlatformSO boss, Material material, int lvlId)
    {
        this.ws = ws;
        this.hpso = hpso;
        //timer = hpso.timeBetweenPlatforms * worldScaleConst / GameManager.Instance.worldSpeed;
        nextDistanceToSpawn = GameManager.Instance.distanceTraveled + hpso.distanceBetweenPlats + platLength - DEFAULT_PLAT_LENGTH;
        readyToSpawn = true;
        this.material = material;
        levelId = lvlId;
        bossHPSO = boss;
    }

    private void Update()
    {
        if(readyToSpawn && nextDistanceToSpawn <= GameManager.Instance.distanceTraveled && spawnBoss)
        {
            SpawnBoss();
            nextDistanceToSpawn = GameManager.Instance.distanceTraveled + bossHPSO.distanceBetweenPlats + platLength - DEFAULT_PLAT_LENGTH;
        }
        if (readyToSpawn && nextDistanceToSpawn <= GameManager.Instance.distanceTraveled && !GameManager.Instance.bossMode)
        {
            if (RollChanceToSpawn(hpso))
            {
                nextDistanceToSpawn = GameManager.Instance.distanceTraveled + hpso.distanceBetweenPlats + platLength - DEFAULT_PLAT_LENGTH;
            }
            else
            {
                nextDistanceToSpawn = GameManager.Instance.distanceTraveled + (hpso.distanceBetweenPlats / 2) + platLength - DEFAULT_PLAT_LENGTH;
            }
            
        }
        else if(readyToSpawn && nextDistanceToSpawn <= GameManager.Instance.distanceTraveled && GameManager.Instance.bossMode && 
            (levelId == GameManager.Instance.bossLevelId || levelId == GameManager.Instance.bossLevelId + 1 || levelId == GameManager.Instance.bossLevelId + 2)){
            print("BOSS PLAT SPAWNING");
            if (RollChanceToSpawn(bossHPSO))
            {
                nextDistanceToSpawn = GameManager.Instance.distanceTraveled + bossHPSO.distanceBetweenPlats + platLength - DEFAULT_PLAT_LENGTH;
            }
            else
            {
                nextDistanceToSpawn = GameManager.Instance.distanceTraveled + (bossHPSO.distanceBetweenPlats / 2) + platLength - DEFAULT_PLAT_LENGTH;
            }
        }
    }

    bool RollChanceToSpawn(HorizontalPlatformSO currentHPSO)
    {
        float chance = Random.Range(0.0f, 1.0f);
        if (chance <= currentHPSO.chanceToSpawn)
        {
            SpawnPlatform(currentHPSO);
            return true;
        }
        return false;
    }

    public void SpawnPlatform(HorizontalPlatformSO currentHPSO)
    {
        //print("Spawn Plat");
        GameObject ass = GameObject.Instantiate(currentHPSO.potentialPlatforms[Random.Range(0, currentHPSO.potentialPlatforms.Length)],
            new Vector3(100, this.hpso.yLevel, 0f), ws.transform.rotation);
        ass.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(r => r.tag == "PlatformMat").material = material;
        platLength = ass.GetComponentsInChildren<Transform>().First(r => r.tag == "Platform").localScale.x;
        ass.transform.parent = ws.gameObject.transform;
        //print("NEW PLAT LENGTH " + platLength);
    }

    public void SpawnBoss()
    {
        if (spawnBoss)
        {
            GameObject ass = GameObject.Instantiate(GameManager.Instance.bossPlat,
            new Vector3(100, this.hpso.yLevel, 0f), ws.transform.rotation);
            ass.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(r => r.tag == "PlatformMat").material = material;
            platLength = ass.GetComponentsInChildren<Transform>().First(r => r.tag == "Platform").localScale.x;
            ass.transform.parent = ws.gameObject.transform;
            spawnBoss = false;
        }
        else
        {
            spawnBoss = true;
        }
    }
}
