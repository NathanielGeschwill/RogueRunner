using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : IEntity
{
    public float speed;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        tagsICanHit = new List<string> { "Enemy" };
        damage = 1;
    }

    public void FireBullet(Vector3 location)
    {
        Vector3 dir = location - transform.position;
        dir = dir.normalized;
        rb.velocity = dir * speed;
    }
}
