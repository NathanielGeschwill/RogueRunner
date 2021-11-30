using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void FireBullet(Vector3 location)
    {
        print(rb);
        Vector3 dir = location - transform.position;
        dir = dir.normalized;
        rb.velocity = dir * speed;
    }
}
