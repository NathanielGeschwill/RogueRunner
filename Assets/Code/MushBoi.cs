using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoi : IEntity
{
    public float speed;
    public bool falling;
    private Animator animator;
    private Rigidbody rb;
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.velocity.y <= -.5 && !falling)
        {
            animator.SetBool("Falling", true);
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            falling = true;
        }
        else if (rb.velocity.y > -.1 && falling)
        {
            animator.SetBool("Falling", false);
            rb.velocity = rb.velocity = Vector3.left * speed;
            falling = true;
        }
        //transform.localPosition += Vector3.right * -1 * speed * Time.deltaTime;
        //transform.localPosition += Vector3.up * -1 * 5 * Time.deltaTime;
    }
}
