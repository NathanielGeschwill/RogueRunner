using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBoi : IEntity
{
    public GameObject bossProjClone;
    List<BossProj> projectiles = new List<BossProj>();
    private GameObject player;
    private Rigidbody rb;
    private float shootTimer;
    private const float SHOOT_TIMER = 1f;
    private bool isAttacking = false;
    private bool alreadyAttacked = false;
    public float targetY;
    private Vector3 targetVector = new Vector3(35f, 0f, 0f);

    public ParticleSystem deathpart;

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


        maxHealth = 5;
        health = maxHealth;

        shootTimer = SHOOT_TIMER;
        for (int i = 0; i < 20; i++)
        {
            GameObject obj = Instantiate(bossProjClone);
            projectiles.Add(obj.GetComponent<BossProj>());
            obj.SetActive(false);
        }


        player = GameManager.Instance.player;
        rb = GetComponent<Rigidbody>();

        GameManager.Instance.PlayAudio(GameManager.AudioClips.BossSpawn);
    }

    private void Update()
    {
        //print("Boss VEL: " + rb.velocity);

        if (isAttacking)
        {
            shootTimer -= Time.deltaTime;
            if(shootTimer <= 0f)
            {
                ShootProj();
                GameManager.Instance.PlayAudio(GameManager.AudioClips.BossAttack);
            }
            //print("Boss Attacking");
            Vector3 currentLookAt = transform.position + (transform.forward * 10);
            transform.LookAt(Vector3.Lerp(currentLookAt, player.transform.position, .0013f));
            //print("PLAYER POS " + player.transform.position);
            //print("CURR POS " + transform.position);
            rb.MovePosition(Vector3.Lerp(transform.position, player.transform.position + targetVector, (.25f) / (player.transform.position - transform.position).magnitude * 2));
        }
    }

    protected override void LoseHealth(object hitObject, int amount)
    {
        
        if(health-1 <= 0)
        {
            GameManager.Instance.ResetBoss();
            GameManager.Instance.PlayAudio(GameManager.AudioClips.BossDeath);
        }
        else
        {
            GameManager.Instance.PlayAudio(GameManager.AudioClips.BossDamaged);

        }
        base.LoseHealth(hitObject, amount);
        Debug.Log("Boss: " + health);
    }

    private void ShootProj()
    {
        //print("SHOOT PROJ");
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (!projectiles[i].gameObject.activeInHierarchy)
            {
                GameManager.Instance.PlayAudio(GameManager.AudioClips.BossAttack);
                //print("FOUND PROJ");
                //print(transform.position);
                projectiles[i].transform.position = transform.position + transform.forward * 10;
                //print("PROJ POS " + projectiles[i].transform.position);
                projectiles[i].transform.rotation = transform.rotation;
                projectiles[i].gameObject.SetActive(true);
                projectiles[i].ResetProj();
                shootTimer = Random.RandomRange(SHOOT_TIMER, SHOOT_TIMER*2);
                return;
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isAttacking && !alreadyAttacked)
        {
            transform.parent = other.gameObject.transform;
            Vector3 dir = player.transform.position - transform.position;
            //vectorFromPlayer = targetVector - new Vector3(-1f, 27f, 0f);
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
