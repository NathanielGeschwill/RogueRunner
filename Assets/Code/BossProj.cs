using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProj : IEntity
{
    
    float deathTimer;
    float DEATH_TIMER_MAX = 2f;
    //public Vector3 dir;
    public Rigidbody rb;
    private float forceMultiplier = 2f;
    public GameObject disperse;

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

    protected override void KillMe()
    {
        gameObject.SetActive(false);
        //GameManager.Instance.PlayAudio(deathSound);
    }

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        tagsICanHit = new List<string> { "Player" };
        damage = 1;
        
    }

    public void ResetProj()
    {
        deathTimer = DEATH_TIMER_MAX + Time.time;
        
        //rb.velocity = transform.forward * forceMultiplier;
        //print("PROJ VEL " + rb.velocity);
        GameManager.Instance.PlayAudio(GameManager.AudioClips.ProjSFX);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * GameManager.Instance.worldSpeed;// * forceMultiplier;
        if (deathTimer < Time.time)
        {
            KillMe();
        }
    }

    protected override void LoseHealth(object hitObject, int amount)
    {
        if (((GameObject)hitObject).GetInstanceID() == gameObject.GetInstanceID())
        {
            if (health - 1 <= 0)
            {
                Instantiate(disperse, transform.position, transform.rotation);
                //GameManager.Instance.ResetBoss();
                //GameManager.Instance.PlayAudio(GameManager.AudioClips.BossDeath);
            }
        }
        
        base.LoseHealth(hitObject, amount);
        //Debug.Log("Boss: " + health);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        print("BOSS PROJ TRIGGERED" + other.name + " " + other);
        if(other.gameObject.tag == "Player")
        {
            GameManager.Instance.PlayAudio(GameManager.AudioClips.ProjHit);
            InvokeHit(other.gameObject, damage);
            Instantiate(disperse, transform.position, transform.rotation);
            KillMe();
        }
    }
}
