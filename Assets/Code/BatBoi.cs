using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBoi : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rb;
    public float forceMultiplier;
    void Start()
    {
        player = GameManager.Instance.player;
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(-GameManager.Instance.worldSpeed,0,0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 dir = player.transform.position - transform.position;
        dir = dir.normalized;
        if (other.gameObject.tag == "Player")
        {
            rb.AddForce(dir * forceMultiplier);
        }
    }
}
