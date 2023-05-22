using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable{
    public void Interact();
}
public class PlayerController : MonoBehaviour
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

    [SerializeField] protected Transform arrowSpawnPoint; 

    [SerializeField] protected ArrowBehaviour arrowPrefab;

    [SerializeField] protected Gadget gadget;

    protected float health;
    protected float speed;
    protected float arrowCharge;
    protected float chargeStart = 0.8f;
    protected float chargeCap = 1.5f;
    protected int arrowCounter = 10;
    private float burningDuration;

    protected bool movementEnabled, aimingEnabled, isReloading, grappled, holdingItem, fullCharge, ropedArrow, onFire, nextLevel, fell;

    [SerializeField] protected GameObject spawnPoint;

    protected Rigidbody rb;
    private Rigidbody pickup;

    private Transform pickupTransform;

    protected ArrowBehaviour currentArrow;

    protected Vector3 movementInput;
    private Vector3 aimDirection, mousePosition;

    protected Camera mainCamera;

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
        spawnPoint = GameObject.FindWithTag("Respawn");
        pickupTransform = transform.GetChild(2);
        rb = GetComponent<Rigidbody>();
        mainCamera = FindObjectOfType<Camera>();
        transform.position = new Vector3(spawnPoint.transform.position.x, 10, spawnPoint.transform.position.z);
        StartCoroutine(MovementEnabler());
        DontDestroyOnLoad(gameObject);
    }

    protected IEnumerator MovementEnabler()
    {
        yield return new WaitForSeconds(2);
        movementEnabled = true;
        aimingEnabled = true;
        nextLevel = false;
    }

    private void Spawn()
    {       
        Reload();
        transform.position = spawnPoint.transform.position;
        fell = false;
        movementEnabled = true;
        aimingEnabled = true;
    }

    public virtual void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "ArrowPickUp")
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
            Debug.Log("Health collected, current health: " + health);
            Destroy(coll.gameObject);
        }

        if (coll.CompareTag("RopePickUp"))
        {
            MakeRoped();
            Debug.Log("You now have a rope!");
            Destroy(coll.gameObject);
        }

        if (coll.CompareTag("Checkpoint"))
        {
            if (spawnPoint != coll.gameObject)
            {
                Debug.Log("New Checkpoint! Now kill yourself");
                spawnPoint = coll.gameObject;
            }
        }

        //this is only to test the hazards. Ideally they're activated when a certain message reaches the client
        if (coll.CompareTag("Hazard"))
        {
            HazardEvent.instance.PickRandomEvent();
            Destroy(coll.gameObject);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        SpeedHandling();

        if (Input.GetMouseButtonUp(0))
        {
            if (holdingItem)
                Throw();
            else
                ShootArrow();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseGadget();
        }

        //testing the final level transition for non-Giant players. This has to be removed
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LevelManager.instance.LoadFinalLevel();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (pickup != null)
            {
                Drop();
            }
            else
                Interact();
        }

        if (transform.position.y <= -3 && !fell)
        {
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
    }

    public virtual void SpeedHandling()
    {
        if (Input.GetMouseButton(1) && aimingEnabled)
        {
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

    private IEnumerator FallCoroutine()
    {
        LevelManager.instance.FallTransition();
        yield return new WaitForSeconds(1f);
        Spawn();
    }

    private void FixedUpdate()
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
                rb.MovePosition(transform.position + movementInput * speed * Time.fixedDeltaTime);
        }

        if (holdingItem)
            pickup.transform.position = pickupTransform.position;
    }

    public virtual void GrappledMovement()
    {
        if (Vector3.Distance(transform.position, movementInput) < 2f)
        {
            grappled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            aimingEnabled = true;
        }
        else
        {
            rb.AddForce((movementInput - transform.position).normalized, ForceMode.VelocityChange);
        }
    }

    public virtual void UseGadget()
    {
        gadget.GadgetAction();
    }

    void Interact()
    {
        Ray r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact();
            }

            if (hitInfo.collider.gameObject.CompareTag("Explosive") || hitInfo.collider.gameObject.CompareTag("Pickup"))
            {
                Pickup(hitInfo.rigidbody);
            }
        }
    }

    void Pickup(Rigidbody obj)
    {
        pickup = obj;
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = false;
        holdingItem = true;
        Debug.Log("Interact with pickup object");
    }

    void Drop()
    {
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = true;
        pickup = null;
        holdingItem = false;
    }

    void Throw()
    {
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = false;
        if (pickup.CompareTag("Explosive"))
        {
            pickup.GetComponent<Explosive>().MakeTrigger();
        }
        pickup.AddForce(transform.forward * arrowSpeed, ForceMode.Impulse);
        pickup = null;
        holdingItem = false;
    }

    protected void RotateLook()
    {
        if (movementInput != Vector3.zero)
        {
            var rot = Quaternion.LookRotation(movementInput, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }
    }

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

    public virtual void Shoot()
    {
        float finalArrowSpeed = arrowSpeed * arrowCharge;
        arrowCharge = chargeStart;
        fullCharge = false;
        arrowCounter--;
        Debug.Log("Arrow Speed is: " + finalArrowSpeed + " and remaining arrows are: " + arrowCounter);
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint);
        currentArrow.transform.localPosition = Vector3.zero;
        if (ropedArrow)
            currentArrow.MakeRoped();
        currentArrow.Shoot(transform.forward * finalArrowSpeed);
        currentArrow.SetOwner(this.gameObject);
        //currentArrow.Shoot(transform.forward, finalArrowSpeed);
        ropedArrow = false;
        currentArrow = null;
        if (arrowCounter > 0)
            Reload();
    }

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

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
        Debug.Log("Took damage: " + damage + ". Health is now: " + health);
    }

    public void TakeFireDamage()
    {
        if (!onFire)
        {
            onFire = true;
            burningDuration = 5f;
            StartCoroutine(FireDamage());
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

    public void PickUpArrow(int arrows)
    {
        arrowCounter += arrows;
    }

    public void MakeRoped()
    {
        ropedArrow = true;
    }

    public virtual void PullTowards(Vector3 destination)
    {
        Debug.Log("You are pulled to: " + destination);
        aimingEnabled = false;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        //rb.isKinematic = true;
        grappled = true;
        movementInput = destination;
    }

    public bool IsGrappled()
    {
        return grappled;
    }

    public virtual void Stun(float stunDuration)
    {
        StartCoroutine(StunCoroutine(stunDuration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        movementEnabled = false;
        aimingEnabled = false;
        yield return new WaitForSeconds(duration);
        movementEnabled = true;
        aimingEnabled = true;
    }

    public void SetSpawnpoint(GameObject spawn)
    {
        spawnPoint = spawn;
    }

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
        transform.position = new Vector3(spawnPoint.transform.position.x, 10, spawnPoint.transform.position.z);
        mainCamera = FindObjectOfType<Camera>();
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

    public bool AtMaxHealth()
    {
        return health.Equals(maxHealth);
    }

    public void Death()
    {
        Debug.Log("RIP");
    }
}
