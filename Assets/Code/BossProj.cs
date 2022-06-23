using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProj : IEntity
{
    
    float deathTimer;
    float DEATH_TIMER_MAX = 2f;
    //public Vector3 dir;
    public Rigidbody rb;
    private float forceMultiplier = 30f;

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
        
        rb.velocity = transform.forward * forceMultiplier;
        //print("PROJ VEL " + rb.velocity);
        GameManager.Instance.PlayAudio(GameManager.AudioClips.ProjSFX);
    }

    // Update is called once per frame
    void Update()
    {
        if(deathTimer < Time.time)
        {
            KillMe();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //print("TRIGGERED");
        foreach (string s in tagsICanHit)
        {
            GameManager.Instance.PlayAudio(GameManager.AudioClips.ProjHit);
        }
    }
}
