using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBoi : IEntity
{
    // Start is called before the first frame update
    void Start()
    {
        tagsICanHit = new List<string> { "Player" };
        damage = 1;
        float randX = Random.Range(-.45f, .45f);
        transform.localPosition += new Vector3(randX, 0, 0);
    }
}
