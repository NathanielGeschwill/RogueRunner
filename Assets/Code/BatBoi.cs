using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBoi : IEntity
{
    private GameObject player;
    private Rigidbody rb;
    private Animator animator;//
    public float forceMultiplier;
    private float dashTimer;
    private const float DASH_TIMER = 2.0f;
    private bool isAttacking = false;
    private bool alreadyAttacked = false;
    private float distToPlayerX = 3.0f;

    void Start()
    {
        player = GameManager.Instance.player;
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(-GameManager.Instance.worldSpeed,0,0); //NEGATIVE WorldSpeed
        dashTimer = DASH_TIMER;
        tagsICanHit = new List<string> { "Player" };
        damage = 1;

        animator = GetComponentInChildren<Animator>();// This code is on the Prefab, the aimator that needs to be accessed is on the rig. Prefab may need to be rearranged to make this work?
        animator.SetBool("isAttacking", false);//
        animator.SetBool("Dash", false);//
        //Debug.Log(animator);

    }

    private void Update()
    {
        if (isAttacking && dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }

        if (isAttacking && dashTimer <= 0)
        {
            Vector3 dir = player.transform.position - transform.position;
            dir = dir.normalized;
            rb.velocity = dir * forceMultiplier;
            dashTimer = DASH_TIMER;
            //transform.LookAt(player.transform);
            animator.SetBool("Dash", true); //
            isAttacking = false;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isAttacking && !alreadyAttacked)
        {
            transform.parent = null;
            Vector3 dir = player.transform.position - transform.position;
            dir = dir.normalized;
            if (dir.x < 0)
            {
                isAttacking = true;
                animator.SetBool("isAttacking", true);//
                alreadyAttacked = true;
                rb.velocity = Vector3.zero;
            }
        }
        else if (other.gameObject.tag == "Player" && alreadyAttacked)
        {
            Vector3 dir = player.transform.position - transform.position;
            if(dir.magnitude <= 2f)
            {
                print("Going to hit player");
                InvokeHit(other.gameObject, damage);
            }
            
        }
    }
}
