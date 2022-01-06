using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoi : IEntity
{
    public float speed;
    public bool falling;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        tagsICanHit = new List<string> { "Player" };
        damage = 1;

        animator = GetComponent<Animator>();
        animator.SetBool("Falling", false);
        animator.SetBool("Death", false);
    }

    private void Update()
    {

        /*if (rb.velocity.y < 0 && falling == false)
        {
           //OnFall?.Invoke();
            falling = true;
            animator.SetBool("Falling", true);

        }*/

        animator.SetBool("Death", false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition += Vector3.right * -1 * speed * Time.deltaTime;
    }
}
