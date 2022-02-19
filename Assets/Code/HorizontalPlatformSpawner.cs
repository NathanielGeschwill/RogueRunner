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

    public void SetWorldSpawn(WorldSpawn ws, HorizontalPlatformSO hpso, Material material)
    {
        this.ws = ws;
        this.hpso = hpso;
        //timer = hpso.timeBetweenPlatforms * worldScaleConst / GameManager.Instance.worldSpeed;
        nextDistanceToSpawn = GameManager.Instance.distanceTraveled + hpso.distanceBetweenPlats;
        readyToSpawn = true;
        this.material = material;
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
        GameObject ass = GameObject.Instantiate(hpso.potentialPlatforms[Random.Range(0, hpso.potentialPlatforms.Length)], new Vector3(100, hpso.yLevel, 0f), ws.transform.rotation);
        ass.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(r => r.tag == "PlatformMat").material = material;
        ass.transform.parent = ws.gameObject.transform;
    }
}
