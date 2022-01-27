using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlatformSpawner : MonoBehaviour
{
    private HorizontalPlatformSO hpso;
    private WorldSpawn ws;
    public bool canSpawn = true;
    private float worldScaleConst = 15.0f;
    //private float worldScaleMod = 10.0f;
    private float timer = 1000;
    private float nextDistanceToSpawn;
    private float distanceTraveled;
    private bool readyToSpawn = false;

    public void SetWorldSpawn(WorldSpawn ws, HorizontalPlatformSO hpso)
    {
        this.ws = ws;
        this.hpso = hpso;
        //timer = hpso.timeBetweenPlatforms * worldScaleConst / GameManager.Instance.worldSpeed;
        nextDistanceToSpawn = GameManager.Instance.distanceTraveled + hpso.distanceBetweenPlats;
        readyToSpawn = true;
    }

    private void Update()
    {
        if(readyToSpawn && nextDistanceToSpawn <= GameManager.Instance.distanceTraveled)
        {
            if (RollChanceToSpawn())
            {
                nextDistanceToSpawn = GameManager.Instance.distanceTraveled + hpso.distanceBetweenPlats;
            }
            else
            {
                nextDistanceToSpawn = GameManager.Instance.distanceTraveled + hpso.distanceBetweenPlats / 2;
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
        var ass = GameObject.Instantiate(hpso.potentialPlatforms[Random.Range(0, hpso.potentialPlatforms.Length)], new Vector3(100, hpso.yLevel, 0f), ws.transform.rotation);
        ass.transform.parent = ws.gameObject.transform;
    }
}
