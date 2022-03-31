using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
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
    public Vector3 rootScale;
    public ParticleSystem jumpPart, landingPart, airjumpPart, damagePart;

    private Animator animator;
    private int ShootLayer;
    private float shoottime = 0;

    public float grav = 20;
    public float jumpVelocity = 20; //how much veritcal velocity is given the player when they jump
    public float jumpDecay = 2.5f; // vertical velocity is reduced to when the player lets go of the jump key
    public float fallMulti = 8f; // multiplier on gravity, to make player feel weightier (later clamped vert velo)
    
    public bool isGrounded = false; //on the ground?
    private bool isHoldingJump = false; //holding jump button?
    //private bool isHoldingJumpforAnim = false;
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

    List<GameObject> currentCol;

    private float playerHurtTimer = .255f;
    private float playerHurtTime = 0;
    private float playerJumppadTimer = .3f;
    private float playerJumppadTime = 0;

    private float ATK_TIME_BETWEEN = 0.225f;
    //private float ATK_TIME_RELOAD = 1.0f;
    private float attackTimer = 0f;
    private bool isReloading = false;
    //private int clipCounter = 0;
    private int clipSlots = 3;

    public delegate void Fall();
    public static event Fall OnFall;
    

    public delegate void Jump();
    public static event Jump OnJump;
    public delegate void Jumppad();
    public static event Jumppad OnJumppad;
    //public static UnityEvent OnJump;


    public delegate void IncreaseUI(string name);
    public static event IncreaseUI OnIncreaseUI;

    public delegate void DecreaseUI(string name);
    public static event DecreaseUI OnDecreaseUI;

    public Text text;
    //private float WALK_SOUND_TIME = .5f;
    //private float walkSoundTimer = 0f;

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    private void OnEnable()
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
    {
        ICollectable.OnCollected += ResolvePickup;
        IEntity.OnHit += LoseHealth;
        IEntity.OnDeath += ResolveDeath;
    }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    private void OnDisable()
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
    {
        ICollectable.OnCollected -= ResolvePickup;
        IEntity.OnHit -= LoseHealth;
        IEntity.OnDeath -= ResolveDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        animator = GetComponentInChildren<Animator>();
        //ShootLayer = animator.GetLayerIndex("Shoot");
        //Debug.Log(animator.layerCount);
        animator.SetBool("grounded", false);

        rootScale = root.transform.localScale;
        currentCol = new List<GameObject>();

        MaxAmmo(bulletPrefab);
        OnDecreaseUI?.Invoke("Bullet");
        OnDecreaseUI?.Invoke("Bullet");
        OnDecreaseUI?.Invoke("Bullet");
        clip.Clear();

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
                GameManager.Instance.PlayAudio(GameManager.AudioClips.AmmoUp);
                break;
            case "Heal":
                GainHealth(gameObject, 1);
                GameManager.Instance.PlayAudio(GameManager.AudioClips.HealthUp);
                break;
        }
    }

    protected override void LoseHealth(object hitObject, int amount)
    {
        if(playerHurtTime < 0)
        {
            if (((GameObject)hitObject).GetInstanceID() == gameObject.GetInstanceID() && !GameManager.Instance.playerTooFast())
            {
                GameManager.Instance.gmScreenShake(.2f, .4f);
                
                OnDecreaseUI?.Invoke("Heart");
                base.LoseHealth(hitObject, amount);
                animator.SetTrigger("hurt");
                GameManager.Instance.PlayAudio(GameManager.AudioClips.PlayerHurt);
                GameManager.Instance.fbm.PlayFeedback("DamageFeedback", damagePart, rootScale, root);
                playerHurtTime = playerHurtTimer;
            }
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
        //print("this happens");
        gm.playerDead = true;
        //gm.SetLoseText();
        //GameManager.Instance.PlayAudio(GameManager.AudioClips.PlayerLava);
    }

    // Update is called once per frame
    void Update()
    {

        //Update velocity just for public visual reference in the editor
        rbVelo = rb.velocity;

        //TESTING tp to top of stage
        if (transform.position.y < -44) //testing
        {
            transform.position = new Vector3(0, 120, 0);
        }


        //If the player is grounded, has remaining jumps, or is currently in CoyoteTime
        if (isGrounded || jumpTemp >= 1 || coyoteTime)
        {
            //JUMP KEY
            if (Input.GetKeyDown(KeyCode.Space))
            {

                if (jumpTemp == jumps)
                {
                    animator.SetTrigger("jump");
                    GameManager.Instance.fbm.PlayFeedback("JumpFeedback", jumpPart, rootScale, root);
                }
                else
                {
                    animator.SetTrigger("airJump");
                    GameManager.Instance.fbm.PlayFeedback("JumpFeedback", airjumpPart, rootScale, root);
                }


                //if player jumps, no longer grounded, immideately add y velocity, and holdingjump is true until key up is registered or max time reached
                isGrounded = false;
                rb.velocity = new Vector3(0, jumpVelocity, 0);
                isHoldingJump = true;
                //isHoldingJumpforAnim = true;

                //number of jumps left is decreased, and we're not entering Coyote time
                jumpTemp -= 1;
                coyoteTime = false;
                coyoteTimer = 0;
                //falling = false;
                OnJump?.Invoke();

            }
        }

        //if Player lets go of the Jump key
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //isHoldingJumpforAnim = false;
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

                //Debug.Log(animator.layerCount);
                //Debug.Log(animator.GetLayerName(1));
                //animator.GetLayerIndex();


                
                // THE ANIM LAYER IS SET IN FootSteps.cs CAUSE FUCK ME
                //Nvm this is irrelevant now, but that's where the code was living, ShotStart() and ShotEnd()
                shoottime = 1;

            }
            animator.SetTrigger("shoot");
            GameManager.Instance.gmScreenShake(.2f, .3f);
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
            else if (!GameManager.Instance.gamePaused)
            {
                GameManager.Instance.Pause();
            }
        }

        text.text = " Dist: " + GameManager.Instance.distanceTraveled.ToString("F1");// + " wspd: " + GameManager.Instance.worldSpeed.ToString("F1") + " toofast: " + GameManager.Instance.playerTooFast();
                                                                                     //text.text = " wspd: " + GameManager.Instance.worldSpeed.ToString("F1") + " Target: "+ GameManager.Instance.targetWorldSpeed.ToString("F1")  + " toofast: " + GameManager.Instance.playerTooFast();

        if (!isHoldingJump) { animator.SetBool("isHoldingJump", false); }
        else { animator.SetBool("isHoldingJump", true); }

        GameManager.Instance.playerGrounded = isGrounded;
        playerHurtTime -= Time.fixedDeltaTime;
        playerJumppadTime -= Time.fixedDeltaTime;


    }



    private void FixedUpdate()
    {
        if(rb.velocity.y < .1 && !isGrounded)
        {
            //OnFall?.Invoke();
            animator.SetTrigger("falling");
        }
        if (jumpPad) //onTriggerEnter(tag=="Jumppad")
        {
            //gm.worldSpeedChange(true, 3); //Jumppad feels like player is being thrown foward.
            rb.velocity = new Vector3(0, jumpVelocity*3f, 0); //Note this is NOT using isHoldingJump, so there is no decay on this jump. This may need special animation
            jumpPad = false;
            isGrounded = false;
            OnJump?.Invoke();
            //OnJumppad?.Invoke();
        }
        if (!isGrounded) //IF NOT GROUNDED
        {
            currentCol.Clear();
            gm.airTime += Time.fixedDeltaTime;
            if (coyoteTime)
            {
                coyoteTimer += Time.fixedDeltaTime;
                if(coyoteTimer >= maxCoyoteTime)
                {
                    coyoteTime = false;
                    coyoteTimer = 0;
                    jumpTemp--; //had chance to jump, decrease number of jumps accordingly
                }
            }
            if (isHoldingJump) //VARIABLE JUMP continue to add force
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
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMulti - 1) * Time.deltaTime;
            OnFall?.Invoke();
        }

        //Cap Fall Speed  (USE CLAMP?)
        if(rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxFallSpeed, rb.velocity.z); 
        }//*/

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name + " " + collision.gameObject.layer.ToString());
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.layer == 11)
        {
            currentCol.Add(collision.gameObject); 
            animator.SetBool("grounded", true);
            if (currentCol.Count < 2) {
                GameManager.Instance.fbm.PlayFeedback("LandingFeedback", landingPart, rootScale, root);
                //animator.SetTrigger("land");
                animator.Play("p_land_001");
                GameManager.Instance.PlayAudio(GameManager.AudioClips.PlayerLand);
            }
            //GameManager.Instance.PlayAudio(GameManager.AudioClips.PlayerLand);
            
            //falling = false; 
            isGrounded = true;
            jumpTemp = jumps;
            gm.airTime = 0;

            #region headbonk
            //Check if the platform player is colliding with is beneath them
            if (collision.contacts[0].point.y < transform.position.y)
            {
            }
            else if (collision.contacts[0].point.y > transform.position.y)
            {
                //Debug.Log("haha");
                //go through platform
            }
            #endregion
        }
    }

    private void OnCollisionStay(Collision collision)
    {
    }

    private void OnCollisionExit(Collision collision)
    {
        currentCol.Remove(collision.gameObject);

        // if you left the platform without jumping
        if (isGrounded && (currentCol.Count < 1))
        {
            isGrounded = false;
            animator.SetBool("grounded", false);
            coyoteTime = true;

            //OnFall?.Invoke();
            //falling = true;
            //animator.SetBool("grounded", false);
        }
    }


#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public void OnTriggerEnter(Collider other)
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
    {
        if(other.gameObject.CompareTag("Jumppad"))
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            JUMPPAD(true, 3);
            GameManager.Instance.PlayAudio(GameManager.AudioClips.Jumppad);
        }
    }

    public void JUMPPAD(bool mul, float x)
    {
        //Debug.Log("<color=red> jumppad </color>", this.gameObject);
        GameManager.Instance.fbm.PlayFeedback("JumpFeedback", jumpPart, rootScale, root);
        jumpPad = true;
        animator.SetTrigger("jumpPad");
        animator.SetBool("grounded", false);
        if (playerJumppadTime > 0 && mul == true) { mul = false; x *= x; /*print(x);*/ }
        gm.worldSpeedChange(mul, x);
        playerJumppadTime = playerJumppadTimer;
    }

    void Attack()
    {
        //play animation

        // detect
        //Collider[] hitEnemies = Physics.OverlapSphere((0, 0, 0), attackRadius, enemyLayer);
    }

}
