using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class Player : IEntity
{
    public GameManager gm; //overlord
    public GameObject bulletPrefab; //bullet prefab to spawn bullets from

    //public List<GameObject> clip;
    public Stack<GameObject> clip = new Stack<GameObject>();

    public Rigidbody rb; //for all your rigidbody needs
    public Vector3 rbVelo;  //for displaying values in editor / debugging

    //For Feedbacks / communicating with FeedbackManager 
    public GameObject root;
    public ParticleSystem jumpPart, landingPart, airjumpPart, damagePart;

    public float grav = 20;
    public float jumpVelocity = 20; //how much veritcal velocity is given the player when they jump
    public float jumpDecay = 2.5f; // vertical velocity is reduced to when the player lets go of the jump key
    public float fallMulti = 8f; // multiplier on gravity, to make player feel weightier (later clamped vert velo)
    
    public bool isGrounded = false; //on the ground?
    private bool isHoldingJump = false; //holding jump button?
    public float maxHoldJumptime = .325f; //as long as you're allow to hold jump button
    private float holdJumpTimer = 0f; //countdown for jump hold
    private float maxFallSpeed = -44; //for vert velo clamp

    //public float airTime = 0; //handled by gm now, mostly for camera offset calcs
    private bool jumpPad = false; //hitting a jumppad, or potentially enemies
    public float jumps = 2; //total jumps
    public float jumpTemp = 2; //jumps remaining
    private bool coyoteTime = false; //forgiveness time
    public float maxCoyoteTime = .225f; //if you take longer than this i won't forgive you
    private float coyoteTimer = 0; //forgiveness countdown

    //
    private float ATK_TIME_BETWEEN = 0.225f;
    private float ATK_TIME_RELOAD = 1.0f;
    private float attackTimer = 0f;
    private bool isReloading = false;
    //private int clipCounter = 0;
    private int clipSlots = 3;

    public delegate void Fall();
    public static event Fall OnFall;

    public delegate void Jump();
    public static event Jump OnJump;
    //public static UnityEvent OnJump;


    public delegate void IncreaseUI(string name);
    public static event IncreaseUI OnIncreaseUI;

    public delegate void DecreaseUI(string name);
    public static event DecreaseUI OnDecreaseUI;

    bool falling = false;

    private void OnEnable()
    {
        ICollectable.OnCollected += ResolvePickup;
        IEntity.OnHit += LoseHealth;
        IEntity.OnDeath += ResolveDeath;
    }

    private void OnDisable()
    {
        ICollectable.OnCollected -= ResolvePickup;
        IEntity.OnHit -= LoseHealth;
        IEntity.OnDeath -= ResolveDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        MaxAmmo(bulletPrefab);
        gm.airTime = 0;
        health = 3;
        maxHealth = 3;
        tagsICanHit = new List<string>{ "Enemy"};
        for(int i = 0; i<health; i++)
        {
            OnIncreaseUI?.Invoke("Heart");
        }
    }

    private bool AddToClip(GameObject newAmmo)
    {
        if(clip.Count < clipSlots)
        {
            //print("adding ammo to clip");
            clip.Push(newAmmo);
            OnIncreaseUI?.Invoke("Bullet");
            return true;
        }
        return false;
    }

    private void MaxAmmo(GameObject newAmmo)
    {
        for(int i=0; i<clipSlots; i++)
        {
            if (!AddToClip(newAmmo))
            {
                return;
            }
        }
    }

    private void ResolvePickup(string pickup)
    {
        switch (pickup)
        {
            case "Bullet":
                AddToClip(bulletPrefab);
                break;
            case "Heal":
                GainHealth(gameObject, 1);
                break;
        }
    }

    protected override void LoseHealth(object hitObject, int amount)
    {
        if (((GameObject)hitObject).GetInstanceID() == gameObject.GetInstanceID())
        {
            OnDecreaseUI?.Invoke("Heart");
            base.LoseHealth(hitObject, amount);
            GameManager.Instance.fbm.PlayFeedback("DamageFeedback", damagePart, root.GetComponent<Transform>(), root);
        }
            
    }

    protected override bool GainHealth(object sender, int amount)
    {
        if(health + 1 <= maxHealth)
        {
            OnIncreaseUI?.Invoke("Heart");
            return base.GainHealth(sender, amount);
        }
        return false;
    }

    protected override void ResolveDeath(object sender, int senderID)
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Update velocity just for public visual reference in the editor
        rbVelo = rb.velocity;

        if(rb.velocity.y < 0 && falling == false)
        {
            OnFall?.Invoke();
            falling = true;
        }
        if(falling && rb.velocity.y > 0.1f)
        {
            OnJump?.Invoke();
            falling = false;
        }

        //TESTING tp to top of stage
        if(transform.position.y < -44) //testing
        {
            transform.position = new Vector3(0, 100, 0);
        }

        //If the player is grounded, has remaining jumps, or is currently in CoyoteTime
        if (isGrounded || jumpTemp >= 1 || coyoteTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //if player jumps, no longer grounded, immideately add y velocity, and holdingjump is true until key up is registered or max time reached
                isGrounded = false;
                rb.velocity = new Vector3(0, jumpVelocity, 0);
                isHoldingJump = true;

                //number of jumps left is decreased, and we're not entering Coyote time
                jumpTemp -= 1;
                coyoteTime = false;
                coyoteTimer = 0;
                falling = false;
                OnJump?.Invoke();
                
                if(!isGrounded)
                {
                    GameManager.Instance.fbm.PlayFeedback("JumpFeedback", airjumpPart, root.GetComponent<Transform>(), root);
                }
                else
                {
                    GameManager.Instance.fbm.PlayFeedback("JumpFeedback", jumpPart, root.GetComponent<Transform>(), root);
                }
            }
        }

        //if Player lets go of the Jump key
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump = false;
            holdJumpTimer = 0;
        }

        //if player left clicks, fire bullet
        if (Input.GetMouseButtonDown(0) && clip.Count > 0 && attackTimer <= 0.0f && !GameManager.Instance.gamePaused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData;
            Vector3 mouseLoc;

            if (Physics.Raycast(ray, out hitData, 1000, LayerMask.GetMask("PlaneLayer")))
            {
                mouseLoc = hitData.point;
                GameObject newBullet = Instantiate(clip.Pop(), transform.position, Quaternion.identity);
                attackTimer = ATK_TIME_BETWEEN;
                newBullet.GetComponent<Bullet>().FireBullet(mouseLoc);
                //print(mouseLoc);
            }

            OnDecreaseUI?.Invoke("Bullet");
        }
        
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
            if (isReloading && attackTimer <= 0.0f)
            {
                isReloading = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.gamePaused && GameManager.Instance.pauseUI.activeInHierarchy)
            {
                GameManager.Instance.Unpause();
            }
            else if(!GameManager.Instance.gamePaused)
            {
                GameManager.Instance.Pause();
            }
        }
    }

    private void FixedUpdate()
    {
        //if player touches a jumppad (may double for bouncing off enemy heads in the future)
        if (jumpPad) //onTriggerEnter(tag=="Jumppad")
        {
            gm.worldSpeedChange(true, 3); //Jumppad feels like player is being thrown foward.
            rb.velocity = new Vector3(0, jumpVelocity*3f, 0); //Note this is NOT using isHoldingJump, so there is no decay on this jump. This may need special animation
            jumpPad = false;
            isGrounded = false;
        }

        if (!isGrounded)
        {
            gm.airTime += Time.fixedDeltaTime;
            if (coyoteTime)
            {
                coyoteTimer += Time.fixedDeltaTime;

                if(coyoteTimer >= maxCoyoteTime)
                {
                    coyoteTime = false;
                    coyoteTimer = 0;
                    jumpTemp--; //had chance to jump, decrease number of jumps accordingly
                    //Debug.Log("shit outta luck"); 
                }
            }

            if (isHoldingJump)
            {
                rb.AddForce(new Vector3(0, jumpVelocity*3f + grav, 0));
                holdJumpTimer += Time.fixedDeltaTime;

                if (holdJumpTimer >= maxHoldJumptime)
                {
                    isHoldingJump = false;
                    holdJumpTimer = 0;
                }
            }
        }
        //GRAVITY
        rb.velocity += Vector3.up * Physics.gravity.y * (grav) * Time.deltaTime;

        //ADDITIONAL GRAVITY
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMulti - 1) * Time.deltaTime; }

        //Cap Fall Speed  (USE CLAMP?)
        if(rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxFallSpeed, rb.velocity.z); }//*/

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            //Check if the platform player is colliding with is beneath them
            if(collision.contacts[0].point.y < transform.position.y)
            {
                isGrounded = true;
                GameManager.Instance.fbm.PlayFeedback("LandingFeedback", landingPart, root.GetComponent<Transform>(), root);
                falling = false;
                jumpTemp = jumps;
                gm.airTime = 0;

            }
            else if(collision.contacts[0].point.y > transform.position.y)
            {
                Debug.Log("haha");
                //go through platform
            }
            
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isGrounded) //if player is not grounded for whatever reason while on the ground
        {
            if(rb.velocity.y == 0) { 
                isGrounded = true;
                falling = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log(isGrounded);
        // if you left the platform without jumping, shit out of luck (add Coyote time here?)
        if(isGrounded)
        {
            isGrounded = false;
            coyoteTime = true;
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Jumppad"))
        {
            Debug.Log("<color=red> jumppad </color>", this.gameObject);
            GameManager.Instance.fbm.PlayFeedback("JumpFeedback", jumpPart, root.GetComponent<Transform>(), root);
            jumpPad = true;
            falling = false;
            OnJump?.Invoke();
            
        }
    }


    void Attack()
    {
        //play animation

        // detect
        //Collider[] hitEnemies = Physics.OverlapSphere((0, 0, 0), attackRadius, enemyLayer);
    }

}
