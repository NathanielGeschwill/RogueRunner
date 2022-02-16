using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoi : IEntity
{
    public float speed;
    public bool falling;
    private Animator animator;
    private Rigidbody rb;
    private float walkSoundTimer = 0f;
    private float WALK_SOUND_TIMER = .5f;
    private bool step2;
    private AudioSource walkSource;
    // Start is called before the first frame update
    void Start()
    {
        //transform.parent = null;
        tagsICanHit = new List<string> { "Player" };
        damage = 1;

        animator = GetComponentInChildren<Animator>();
        animator.SetBool("Falling", false);
        animator.SetBool("Death", false);
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.left * speed;
        //print(rb.velocity);
        deathSound = GameManager.AudioClips.MushDeath;
        walkSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

        /*if (rb.velocity.y < 0 && falling == false)
        {
           //OnFall?.Invoke();
            falling = true;
            animator.SetBool("Falling", true);

        }*/

        //animator.SetBool("Death", false);

        if (!falling)
        {
            /*
            walkSoundTimer += Time.deltaTime;
            if(walkSoundTimer >= WALK_SOUND_TIMER)
            {
                if (step2)
                {
                    walkSource.PlayOneShot(GameManager.Instance.audioClips[(int)GameManager.AudioClips.MushStep2]);
                }
                else
                {
                    walkSource.PlayOneShot(GameManager.Instance.audioClips[(int)GameManager.AudioClips.MushStep1]);
                }
                walkSoundTimer = 0f;
            }*/
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rb.velocity.y < -24) { rb.velocity = new Vector3(rb.velocity.x, -24, rb.velocity.z); }
        else if (falling) { rb.velocity += Vector3.up * Physics.gravity.y * (7) * Time.deltaTime; }
        else { rb.velocity = Vector3.left * speed; }


        if (rb.velocity.y <= -.5 && !falling)
        {

        }

        //transform.localPosition += Vector3.right * -1 * speed * Time.deltaTime;
        //transform.localPosition += Vector3.up * -1 * 5 * Time.deltaTime;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        print("wow");
        //base.OnTriggerEnter(other);
        print(other.gameObject.name.ToString());
        if (other.gameObject.CompareTag("Bullet"))
        {
            print("This is a bullet");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            print("bullet type shit");

            LoseHealth(gameObject, 1);
        }
        //print("wowc");
        //Debug.Log("Collidiing " + collision.gameObject.name);
        if(collision.gameObject.layer == 11 || collision.gameObject.CompareTag("Platform"))
        {
            animator.SetBool("Falling", false);
            rb.velocity = Vector3.left * speed;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            falling = false;
        }
        
        foreach (string s in tagsICanHit)
        {
            if (collision.gameObject.CompareTag(s) && damage > 0)
            {
                print("GOING INVOKE " + collision.gameObject);
                FireOnHit(collision.gameObject);
                break;
            }
            else
            {
                //print("didn't find");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 11 || collision.gameObject.CompareTag("Platform"))
        {
            animator.SetBool("Falling", true);
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            falling = true;
        }
    }

}
