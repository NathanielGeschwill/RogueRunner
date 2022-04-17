using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBoi : IEntity
{
    private GameObject player;
    private Rigidbody rb;
    //private Animator animator;//
    public float forceMultiplier;
    //private float dashTimer;
    //private const float DASH_TIMER = 2.0f;
    private bool isAttacking = false;
    private bool alreadyAttacked = false;
    //private float distToPlayerX = 3.0f;
    private Vector3 vectorFromPlayer;

    public ParticleSystem deathpart;
    public GameObject root;
    private Vector3 rootScale;

    //private bool lockOn = false;
    //private float lockOnTimer;

    //private float flapSoundTimer = 0f;
    //private float FLAP_SOUND_TIMER = 1f;

    //private AudioSource flapSource;

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

    void Start()
    {
        player = GameManager.Instance.player;
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(-GameManager.Instance.worldSpeed, 0, 0); //NEGATIVE WorldSpeed
        //dashTimer = DASH_TIMER;
        tagsICanHit = new List<string> { "Player" };
        damage = 1;
        rootScale = root.transform.localScale;

        //animator = GetComponentInChildren<Animator>();// This code is on the Prefab, the aimator that needs to be accessed is on the rig. Prefab may need to be rearranged to make this work?
        //animator.SetBool("isAttacking", false);//
        //animator.SetBool("Dash", false);//
        //Debug.Log(animator);
        deathSound = GameManager.AudioClips.BatDeath;
        //flapSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //if (lockOn) { lockOnTimer += Time.fixedDeltaTime; }

        if (isAttacking)
        {
            //dashTimer -= Time.deltaTime;
            //rb.velocity = new Vector3(0, player.GetComponent<Player>().rb.velocity.y, 0);

            //Further bat is from you, the faster the locked on Animation plays
            //if (Vector3.Distance(transform.position, player.transform.position + vectorFromPlayer) > 2.1f)
            //{ animator.SetFloat("DashDist", Mathf.Clamp(Vector3.Distance(transform.position, player.transform.position + vectorFromPlayer), 1, 3.6f)); }
            //else { animator.SetFloat("DashDist", 1); }

            rb.MovePosition(Vector3.Lerp(transform.position, player.transform.position + vectorFromPlayer, (.25f) / (player.transform.position - transform.position).magnitude * 2));
            //float f = ((lockOnTimer + .01f) * .05f); Mathf.Clamp(f, 0.01f, .25f); 
            //rb.MovePosition(Vector3.Lerp(transform.position, player.transform.position+vectorFromPlayer,  f / (player.transform.position - transform.position).magnitude * 2));
        }

        //if (isAttacking && dashTimer <= 0)
        //{
        //    Vector3 dir = player.transform.position - transform.position;
        //    dir = dir.normalized;
        //    rb.velocity = dir * forceMultiplier;
        //    dashTimer = DASH_TIMER;
        //    //transform.LookAt(player.transform);
        //    animator.SetBool("Dash", true); //
        //    isAttacking = false;
        //    GameManager.Instance.PlayAudio(GameManager.AudioClips.BatDash);
        //}

        if (!isAttacking)
        {/*
            flapSoundTimer += Time.deltaTime;
            if(flapSoundTimer >= FLAP_SOUND_TIMER)
            {
                flapSource.PlayOneShot(GameManager.Instance.audioClips[(int)GameManager.AudioClips.BatFlap]);
                flapSoundTimer = 0;
            }*/
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isAttacking && !alreadyAttacked)
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
                alreadyAttacked = true;
                rb.velocity = Vector3.zero;
            }
        }
        //else if (other.gameObject.tag == "Player" && alreadyAttacked)
        //{
        //    Vector3 dir = player.transform.position - transform.position;
        //    if (dir.magnitude <= 2f)
        //    {
        //        //print("Going to hit player");
        //        InvokeHit(other.gameObject, damage);
        //    }

        //}
    }

    //public void myDamage()
    //{
    //    GameManager.Instance.fbm.PlayFeedback("DamageFeedback", deathpart, rootScale, root);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player" && isAttacking)
    //    {
    //        animator.SetFloat("DashDist", 1);
    //        lockOn = false;
    //        lockOnTimer = 0.0f;
    //        transform.parent = null;
    //        isAttacking = false;
    //        rb.velocity = new Vector3(-GameManager.Instance.worldSpeed * .8f, 0, 0);
    //    }
    //}
}
