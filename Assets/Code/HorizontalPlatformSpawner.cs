using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HorizontalPlatformSpawner : MonoBehaviour
{
    private HorizontalPlatformSO hpso;
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

    public void SetWorldSpawn(WorldSpawn ws, HorizontalPlatformSO hpso, Material material, int lvlId)
    {
        this.ws = ws;
        this.hpso = hpso;
        //timer = hpso.timeBetweenPlatforms * worldScaleConst / GameManager.Instance.worldSpeed;
        nextDistanceToSpawn = GameManager.Instance.distanceTraveled + hpso.distanceBetweenPlats + platLength - DEFAULT_PLAT_LENGTH;
        readyToSpawn = true;
        this.material = material;
        levelId = lvlId;
    }

    private void Update()
    {
        if(readyToSpawn && nextDistanceToSpawn <= GameManager.Instance.distanceTraveled && (!GameManager.Instance.bossMode || (levelId == 0 || levelId == 1)))
        {
            if (RollChanceToSpawn())
            {
                nextDistanceToSpawn = GameManager.Instance.distanceTraveled + hpso.distanceBetweenPlats + platLength - DEFAULT_PLAT_LENGTH;
            }
            else
            {
                nextDistanceToSpawn = GameManager.Instance.distanceTraveled + (hpso.distanceBetweenPlats / 2) + platLength - DEFAULT_PLAT_LENGTH;
            }
            
        }
    }

    bool RollChanceToSpawn()
    {
        float chance = Random.Range(0.0f, 1.0f);
        if (chance <= hpso.chanceToSpawn)
        {
            SpawnPlatform();
            return true;
        }
        return false;
    }

    public void SpawnPlatform()
    {
        //print("Spawn Plat");
        GameObject ass = GameObject.Instantiate(hpso.potentialPlatforms[Random.Range(0, hpso.potentialPlatforms.Length)], new Vector3(100, hpso.yLevel, 0f), ws.transform.rotation);
        ass.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(r => r.tag == "PlatformMat").material = material;
        platLength = ass.GetComponentsInChildren<Transform>().First(r => r.tag == "Platform").localScale.x;
        ass.transform.parent = ws.gameObject.transform;
        //print("NEW PLAT LENGTH " + platLength);
    }
}
