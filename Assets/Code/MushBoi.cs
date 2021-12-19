using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoi : IEntity
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        tagsICanHit = new List<string> { "Player" };
        damage = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition += Vector3.right * -1 * speed * Time.deltaTime;
    }
}
