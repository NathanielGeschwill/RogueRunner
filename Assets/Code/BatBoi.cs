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
    private Vector3 vectorFromPlayer;

    private void OnEnable()
    {
        Player.OnJumppad += KillMe;
    }

    private void OnDisable()
    {
        Player.OnJumppad -= KillMe;
    }

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
            //rb.velocity = new Vector3(0, player.GetComponent<Player>().rb.velocity.y, 0);
            rb.MovePosition(Vector3.Lerp(transform.position, player.transform.position+vectorFromPlayer, .5f/(player.transform.position - transform.position).magnitude * 2));
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
            transform.parent = other.gameObject.transform;
            Vector3 dir = player.transform.position - transform.position;
            vectorFromPlayer = transform.position - player.transform.position;
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && isAttacking)
        {
            transform.parent = null;
            isAttacking = false;
            rb.velocity = new Vector3(-GameManager.Instance.worldSpeed, 0, 0);
        }
    }
}
