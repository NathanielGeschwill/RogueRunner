using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBoi : IEntity
{
    private GameObject player;
    private Rigidbody rb;
    private Vector3 vectorFromPlayer;
    private bool isAttacking = false;

    override protected void OnEnable()
    {
        Player.OnJumppad += KillMe;
        base.OnEnable();
    }

    override protected void OnDisable()
    {
        Player.OnJumppad -= KillMe;
        base.OnDisable();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(-GameManager.Instance.worldSpeed, 0, 0); //NEGATIVE WorldSpeed
        tagsICanHit = new List<string> { "Player" };
        damage = 1;


        
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(Vector3.Lerp(transform.position, player.transform.position + vectorFromPlayer, (.25f) / (player.transform.position - transform.position).magnitude * 2));
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isAttacking)
        {
            //lockOn = true;
            transform.parent = other.gameObject.transform;
            Vector3 dir = player.transform.position - transform.position;
            vectorFromPlayer = transform.position - player.transform.position;
            dir = dir.normalized;
            if (dir.x < 0)
            {
                isAttacking = true;
                //animator.SetBool("isAttacking", true);//
                //alreadyAttacked = true;
                rb.velocity = Vector3.zero;
            }
        }
    }
}
