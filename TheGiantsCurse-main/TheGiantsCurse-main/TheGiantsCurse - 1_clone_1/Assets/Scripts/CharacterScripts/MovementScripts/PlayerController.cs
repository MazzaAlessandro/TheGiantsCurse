using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

//The basic script that handles everything about the player. It is overridden to create the various characters (beside the Giant)
interface IInteractable{
    public void Interact();
}
public class PlayerController : NetworkBehaviour
{
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] private float fallDamage = 1f;
    [SerializeField] private float healthRegain = 5f;
    [SerializeField] protected float movementSpeed = 8f;
    [SerializeField] protected float aimingSpeed = 4f;
    [SerializeField] private float grappleSpeed = 8f;
    [SerializeField] protected float arrowSpeed = 20f;
    [SerializeField] private float turnSpeed = 720;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float interactRange = 1f;

    [SerializeField] private int serverCode;
    [SerializeField] public int playerCode;

    [SerializeField] protected Transform arrowSpawnPoint; 

    [SerializeField] protected ArrowBehaviour arrowPrefab;

    [SerializeField] protected Gadget gadget;

    [SerializeField] protected PlayerNetworkActions networkActions;

    protected float health;
    protected float speed;
    protected float arrowCharge;
    protected float chargeStart = 0.8f;
    protected float chargeCap = 1.5f;
    protected int arrowCounter = 10;
    private float burningDuration;

    protected bool movementEnabled, aimingEnabled, isReloading, grappled, holdingItem, fullCharge, ropedArrow, onFire, nextLevel, fell;
    private bool initSpawn = false;
    [SerializeField] protected bool dead;

    [SerializeField] protected GameObject spawnPoint;
    [SerializeField] protected GameObject cameraPrefab;

    protected Rigidbody rb;
    private Rigidbody pickup;

    private Transform pickupTransform, throwTransform;

    protected ArrowBehaviour currentArrow;

    protected Vector3 movementInput, grappleDestination;
    private Vector3 aimDirection, mousePosition;

    protected GameObject cameraInstance;
    [SerializeField] protected Camera mainCamera;

    protected LineRenderer lineRenderer;

    [SerializeField] private AudioClip fall;

    [SerializeField] protected Animator animator;

    public Healthbar healthbar;
    public ArrowCounter arrowUI;

    //When spawning on other clients, if it is not the owner of the objectdisables the HUD and disables this
    //If it's the owner of the object, instantiates his own camera to follow him
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            foreach(Canvas i in this.gameObject.GetComponentsInChildren<Canvas>())
            {
                i.enabled = false;
            }
            if (cameraInstance != null)
                cameraInstance.SetActive(false);
            enabled = false;
        }
        else
        {
            Debug.Log("Set up camera");
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
            //transform.position = new Vector3(0, 10, 0);
            //rb.useGravity = true;
            /*cameraInstance = Instantiate(cameraPrefab, this.transform);
            mainCamera = cameraInstance.GetComponentInChildren<Camera>();
            cameraInstance.GetComponent<CameraFollow>().ChangeFollow(this.gameObject);
            if (GameObject.FindWithTag("tmpCam")!=null)
                GameObject.FindWithTag("tmpCam").SetActive(false);
            HazardEvent.instance.SetCamera(cameraInstance.transform.GetChild(0).gameObject);*/
        }
            //mainCamera.GetComponentInParent<CameraFollow>().ChangeFollow(this.gameObject);

    }

    private void OnDisable()
    {
        Destroy(cameraInstance);
    }
    //Sets up the various booleans and values
    private void Awake()
    {
        health = maxHealth;
        fell = false;
        onFire = false;
        grappled = false;
        ropedArrow = false;
        movementEnabled = false;
        aimingEnabled = false;
        fullCharge = false;
        dead = false;
        healthbar.SetMaxHealth(maxHealth);
        arrowUI.UpdateArrowNumber(arrowCounter);
        arrowUI.SetRopeImage(false);
        pickupTransform = transform.GetChild(2);
        throwTransform = transform.GetChild(3);
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        //transform.position = new Vector3(spawnPoint.transform.position.x, 10, spawnPoint.transform.position.z);
        /*transform.position = new Vector3(0, 0, 0);
        animator.SetBool("isFalling", true);
        StartCoroutine(MovementEnabler());
        DontDestroyOnLoad(gameObject);*/
    }

    private void Start()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        //transform.position = new Vector3(spawnPoint.transform.position.x, 10, spawnPoint.transform.position.z);
        //transform.position = new Vector3(0, 0, 0);
        animator.SetBool("isFalling", true);
        StartCoroutine(MovementEnabler());
        cameraInstance = Instantiate(cameraPrefab, this.transform);
        mainCamera = cameraInstance.GetComponentInChildren<Camera>();
        cameraInstance.GetComponent<CameraFollow>().ChangeFollow(this.gameObject);
        if (GameObject.FindWithTag("tmpCam") != null)
            GameObject.FindWithTag("tmpCam").SetActive(false);
        //HazardEvent.instance.SetCamera(cameraInstance.transform.GetChild(0).gameObject);
    }

    //Coroutine that enables all movements after a bit of time
    //movement is locked during spawn to preven player from moving during loading of scenes
    protected IEnumerator MovementEnabler()
    {
        yield return new WaitForSeconds(1.25f);
        animator.SetBool("isFalling", false);
        movementEnabled = true;
        aimingEnabled = true;
        nextLevel = false;
    }

    //Called when a player falls to reset him to a spawnpoint transform
    private void Spawn()
    {       
        Reload();
        transform.position = spawnPoint.transform.position;
        transform.rotation = Quaternion.identity;
        fell = false;
        movementEnabled = true;
        aimingEnabled = true;
        animator.SetBool("isFalling", false);
    }

    //Handles the various interactions with pickups and checkpoints
    public virtual void OnTriggerEnter(Collider coll)
    {
        if (!dead)
        {
            if (coll.gameObject.tag == "ArrowPickUp")
            {
                PickUpArrow(1);
                Debug.Log("Arrow collected, current arrow count: " + arrowCounter);
                Destroy(coll.gameObject);
            }

            if (coll.CompareTag("HealthPickUp") && !health.Equals(maxHealth))
            {
                health += healthRegain;
                if (health >= maxHealth)
                    health = maxHealth;
                healthbar.SetHealth(health);
                Debug.Log("Health collected, current health: " + health);
                Destroy(coll.gameObject);
            }

            if (coll.CompareTag("RopePickUp"))
            {
                MakeRoped();
                Debug.Log("You now have a rope!");
                Destroy(coll.gameObject);
            }

            if (coll.CompareTag("Cooldown"))
            {
                gadget.ReduceCooldown(10f);
                Destroy(coll.gameObject);
            }

            if (coll.CompareTag("Checkpoint"))
            {
                if (spawnPoint != coll.gameObject)
                {
                    spawnPoint = coll.gameObject;
                    if (coll.gameObject.GetComponent<InitCheckpoint>() != null && !initSpawn)
                    {
                        playerCode = coll.gameObject.GetComponent<InitCheckpoint>().respectivePlayer;
                        MatchManager.instance.SetLocalPlayer(this);
                        initSpawn = true;
                    }
                }
            }

            if (coll.CompareTag("Hazard"))
            {
                HazardEvent.instance.PickRandomEvent(playerCode);
                Destroy(coll.gameObject);
            }
        }
        
    }

    // Update is used for getting player input. It may get reworked if needed
    // Update is called once per frame
    private void Update()
    {
        if (!dead)
        {
            SpeedHandling();

            if (Input.GetMouseButtonUp(1))
                animator.SetBool("isAiming", false);

            if (Input.GetMouseButtonUp(0))
            {
                if (networkActions.holdingItem)
                    Throw();
                else
                    ShootArrow();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                UseGadget();
            }

            //This input is used for testing certain methods during programming, it can be easily removed
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                //MatchManager.instance.Test(playerCode);
                //networkActions.Test(playerCode);
                //MoveToFinalTrack();
                //NetworkMatchManager.instance.Test();
                //LevelManager.instance.LoadFinalLevel();
                //HazardEvent.instance.Earthquake();
                //Death();
                MatchManager.instance.EndReached(playerCode, NetworkManager.LocalClientId);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (networkActions.pickup != null)
                {
                    Drop();
                }
                else
                    Interact();
            }

            if (transform.position.y < -0.2f)
            {
                animator.SetBool("isFalling", true);
            }
            else
                animator.SetBool("isFalling", false);

            if (transform.position.y <= -3 && !fell)
            {
                //SoundManager.instance.PlayEffect(fall);
                TakeDamage(fallDamage);
                fell = true;
                Debug.Log("Current health: " + health);
                if (health > 0)
                    StartCoroutine(FallCoroutine());
                else
                {
                    Death();
                }

            }

            if (pickup == null)
            {
                holdingItem = false;
            }

            if (grappled)
            {
                Vector3[] positions = new Vector3[]
                {
                grappleDestination,
                transform.position
                };

                lineRenderer.SetPositions(positions);
            }
        }
    }

    //handles movement speed based on input, it had to be split because we needed to override this on ChasmController
    public virtual void SpeedHandling()
    {
        if (Input.GetMouseButton(1) && aimingEnabled)
        {
            animator.SetBool("isAiming", true);
            float dir = Vector3.Dot(transform.forward, movementInput);
            animator.SetFloat("dir", dir);
            speed = aimingSpeed;
            Aiming();
        }
        else
        {
            arrowCharge = chargeStart;
            speed = movementSpeed;
            if (movementEnabled)
                RotateLook();
        }
    }

    //handles the respawn of players
    private IEnumerator FallCoroutine()
    {
        //LevelManager.instance.FallTransition();
        TransitionHandler.instance.CloseAndOpen(1.5f);
        yield return new WaitForSeconds(1f);
        Spawn();
        
    }

    //Handles movement inputs. It is in FixedUpdate instead of normal update because FixedUpdate handles physics more accurately
    private void FixedUpdate()
    {
        if (!dead)
        {
            if (grappled)
            {
                GrappledMovement();
            }
            else
            {
                float horizontal = Input.GetAxisRaw("Horizontal");
                float vertical = Input.GetAxisRaw("Vertical");
                movementInput = new Vector3(horizontal, 0, vertical);

                if (movementEnabled)
                {
                    if (movementInput != Vector3.zero)
                    {
                        animator.SetBool("isMoving", true);
                    }
                    else
                        animator.SetBool("isMoving", false);
                    rb.MovePosition(transform.position + movementInput * speed * Time.fixedDeltaTime);
                }

            }

            //if (holdingItem) pickup.transform.position = pickupTransform.position;
        }
    }

    //Moves the player to a grappling point
    public virtual void GrappledMovement()
    {
        if (Vector3.Distance(transform.position, movementInput) < 2f)
        {
            grappled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            aimingEnabled = true;
            lineRenderer.enabled = false;
        }
        else
        {
            rb.AddForce((movementInput - transform.position).normalized, ForceMode.VelocityChange);
        }
    }

    //Calls the Gadget action. It is overridden by ReaperController because it has multiple GadgetActions
    public virtual void UseGadget()
    {
        //gadget.GadgetAction();
        networkActions.GadgetAction();
    }

    //Either interacts with interactable items or pick up explosive barrels
    void Interact()
    {
        networkActions.Interact();
        /*Ray r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                Debug.Log("interact");
                interactObj.Interact();
            }

            if (hitInfo.collider.gameObject.CompareTag("Explosive") || hitInfo.collider.gameObject.CompareTag("Pickup"))
            {
                Pickup(hitInfo.rigidbody);
            }
        }*/
    }

    //Make the pickup follow the player
    void Pickup(Rigidbody obj)
    {
        pickup = obj;
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = false;
        holdingItem = true;
        Debug.Log("Interact with pickup object");
    }

    //Stops the pickup from following the player anymore
    void Drop()
    {
        StartCoroutine(StopMovement(0.25f));
        networkActions.Drop();
        //pickup.transform.position = throwTransform.position;
        /*pickup.transform.position = pickupTransform.position;
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = true;
        pickup = null;
        holdingItem = false;*/
    }

    //Places the pickup in front of the player and applies force to it
    void Throw()
    {
        animator.SetTrigger("throw");
        StartCoroutine(StopMovement(0.25f));
        networkActions.Throw();
        /*pickup.transform.position = throwTransform.position;
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = false;
        if (pickup.CompareTag("Explosive"))
        {
            pickup.GetComponent<Explosive>().MakeTrigger();
        }
        pickup.AddForce(transform.forward * arrowSpeed, ForceMode.Impulse);
        pickup = null;
        holdingItem = false;*/
    }

    //Briefly stops the player from moving. Needed for game balance, to prevent some problems in physics interactions and make animations feel more appropriate
    public virtual IEnumerator StopMovement(float duration)
    {
        movementEnabled = false;
        aimingEnabled = false;
        yield return new WaitForSeconds(duration);
        movementEnabled = true;
        aimingEnabled = true;
    }

    //Turns the player in the direction of the movement
    protected void RotateLook()
    {
        if (movementInput != Vector3.zero)
        {
            var rot = Quaternion.LookRotation(movementInput, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }
    }


    //When aiming, makes the player face the direction of the pointer, additionally it calls ChargeArrow to add force to the arrow
    protected void Aiming()
    {
        ChargeArrow();

        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLenght;

        if(groundPlane.Raycast(cameraRay, out rayLenght))
        {
            mousePosition = cameraRay.GetPoint(rayLenght);
            Debug.DrawLine(cameraRay.origin, mousePosition, Color.blue);

            aimDirection = new Vector3(mousePosition.x, transform.position.y, mousePosition.z);
            transform.LookAt(aimDirection);
        }
    }

    //The longer this is called, the stronger the arrow charge (up to a cap value)
    public virtual void ChargeArrow()
    {
        if (!fullCharge)
        {
            arrowCharge += Time.deltaTime;
            if (arrowCharge >= chargeCap)
            {
                arrowCharge = chargeCap;
                Debug.Log("Maximum charge reached at: " + arrowCharge);
                fullCharge = true;
            }
        }
    }

    //If the player is not reloading and if he has arrows left, it calls the Shoot() function
    void ShootArrow()
    {
        if (isReloading)
        {
            Debug.Log("You are reloading");
            return; 
        }
        if (arrowCounter >= 1)
        {
            Shoot();
        } 
        else
        {
            Debug.Log("Out of arrows");
        }
    }

    //Instantiates the arrows, adds force to it and the rope if one was picked up
    //It is overridden by CallystoController, that makes the arrow on fire if the arrow is at maximum charge 
    public virtual void Shoot()
    {
        float finalArrowSpeed = arrowSpeed * arrowCharge;
        arrowCharge = chargeStart;
        fullCharge = false;
        arrowCounter--;
        arrowUI.UpdateArrowNumber(arrowCounter);
        Debug.Log("Arrow Speed is: " + finalArrowSpeed + " and remaining arrows are: " + arrowCounter);
        animator.SetTrigger("Shoot");
        networkActions.ShootArrow(finalArrowSpeed, ropedArrow, false);
        //currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint);
        //currentArrow.transform.localPosition = Vector3.zero;
        if (ropedArrow)
        {
            //currentArrow.MakeRoped();
            arrowUI.SetRopeImage(false);
            aimingEnabled = false;
            movementEnabled = false;
        }
            
        //currentArrow.Shoot(transform.forward * finalArrowSpeed);
        //currentArrow.SetOwner(this.gameObject);
        //currentArrow.Shoot(transform.forward, finalArrowSpeed);
        ropedArrow = false;
        //currentArrow = null;
        if (arrowCounter > 0)
            Reload();
    }

    //Prevents the player from shooting continously
    protected void Reload()
    {
        if (isReloading) 
            return;
        isReloading = true;
        StartCoroutine(ReloadAfterTime());
    }

    private IEnumerator ReloadAfterTime()
    {
        yield return new WaitForSeconds(reloadTime);
        //currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint);
        //currentArrow.transform.localPosition = Vector3.zero;
        isReloading = false;
    }

    //Subtracts health. If health is zero, the player dies
    public virtual void TakeDamage(float damage)
    {
        if (!dead)
        {
            health -= damage;
            healthbar.SetHealth(health);
            if (health <= 0)
            {
                Death();
            }
            Debug.Log("Took damage: " + damage + ". Health is now: " + health);
        }
    }

    //Makes the player take damage over time for a small duration
    public void TakeFireDamage()
    {
        if (!dead)
        {
            if (!onFire)
            {
                onFire = true;
                burningDuration = 5f;
                StartCoroutine(FireDamage());
            }
        }
        
    }

    private IEnumerator FireDamage()
    {
        TakeDamage(1);
        burningDuration -= 1;
        yield return new WaitForSeconds(1f);
        if (burningDuration == 0)
            onFire = false;
        else if (burningDuration > 0)
            StartCoroutine(FireDamage());
    }

    //Adds an arrow to the counter
    public void PickUpArrow(int arrows)
    {
        arrowCounter += arrows;
        arrowUI.UpdateArrowNumber(arrowCounter);
    }

    //Adds the rope attribute to the next arrow shot
    public void MakeRoped()
    {
        ropedArrow = true;
        arrowUI.SetRopeImage(true);
    }

    //Set up for the grappled movement
    public virtual void PullTowards(Vector3 destination)
    {
        Debug.Log("You are pulled to: " + destination);
        aimingEnabled = false;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        lineRenderer.enabled = true;
        grappleDestination = destination;
        //rb.isKinematic = true;
        grappled = true;
        movementInput = destination;
    }

    public bool IsGrappled()
    {
        return grappled;
    }

    //stops the player for a bit of time, usually called when hit by other players
    public virtual void Stun(float stunDuration)
    {
        if(!dead || !grappled)
            StartCoroutine(StunCoroutine(stunDuration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        movementEnabled = false;
        aimingEnabled = false;
        animator.SetTrigger("Stun");
        yield return new WaitForSeconds(duration);
        movementEnabled = true;
        aimingEnabled = true;
    }

    //Allows the player to update his spawnpoint with a check point
    public void SetSpawnpoint(GameObject spawn)
    {
        spawnPoint = spawn;
    }

    //The next four methods are needed to properly handle the player behaviour on level transition
    public void EnterLevel()
    {
        rb.useGravity = false;
        movementEnabled = false;
        aimingEnabled = false;
        StartCoroutine(NewLevel());
    }

    protected virtual IEnumerator NewLevel()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Set up camera");
        /*cameraInstance = Instantiate(cameraPrefab, null);
        mainCamera = cameraInstance.GetComponentInChildren<Camera>();
        cameraInstance.GetComponent<CameraFollow>().ChangeFollow(this.gameObject);
        if (GameObject.FindWithTag("tmpCam") != null)
            GameObject.FindWithTag("tmpCam").SetActive(false);*/
        transform.position = new Vector3(spawnPoint.transform.position.x, 10, spawnPoint.transform.position.z);
        //mainCamera = FindObjectOfType<Camera>();
        rb.useGravity = true;
        StartCoroutine(MovementEnabler());
    }

    public bool CanExit()
    {
        return nextLevel;
    }

    public void TurnOffExit()
    {
        nextLevel = true;
    }

    //Checks if the player is at max health
    public bool AtMaxHealth()
    {
        return health.Equals(maxHealth);
    }

    //The player dies. If we are in the final track, it signals to the others that he died
    public void Death()
    {
        animator.SetTrigger("Death");
        dead = true;
        movementEnabled = false;
        aimingEnabled = false;
        networkActions.Death();
        MatchManager.instance.PlayerDeath(NetworkManager.LocalClientId);
        /*if (FinalTrackManagement.instance != null)
        {
            if (IsOwner)
            {
                FinalTrackManagement.instance.PlayerDied(this.gameObject);
                DeathAction();
                Debug.Log("RIP");
            }

        }
        else
            DeathAction();*/
        
    }

    public void Defeat()
    {
        animator.SetTrigger("Death");
        dead = true;
        movementEnabled = false;
        aimingEnabled = false;
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());

    }

    //Handles the player death
    /*public void DeathAction()
    {
        
    }*/

    //Handles player victory, communicating to the others that one player won
    public void Victory()
    {
        dead = true;
        movementEnabled = false;
        aimingEnabled = false;
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());
        if (MatchManager.instance != null)
        {
            if (IsOwner)
            {
                //FinalTrackManagement.instance.PlayerEscaped(this.gameObject);
                //LevelManager.instance.Victory();
                MatchManager.instance.PlayerEscaped(playerCode);
                Debug.Log("Escaped");
                //Destroy(this.gameObject);
            }
        }
    }

    public void EnableMovement()
    {
        movementEnabled = true;
        aimingEnabled = true;
    }

    public int GetServerCode()
    {
        return serverCode;
    }

    public void MoveToFinalTrack()
    {
        Light globalLight = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light>();

        FinalTrackManagement.instance.AssignSpawn(this, playerCode);
        TransitionHandler.instance.CloseAndOpen(2f);

        if (globalLight.intensity < 1)
        {
            globalLight.intensity = 1;
        }

        EnterLevel();
    }

    public void SetPlayerCode(int code)
    {
        playerCode = code;
    }
}
