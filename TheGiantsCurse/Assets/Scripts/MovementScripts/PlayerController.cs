using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable{
    public void Interact();
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    [SerializeField] private float fallDamage = 1f;
    [SerializeField] private float healthRegain = 5f;
    [SerializeField] private float movementSpeed = 8f;
    [SerializeField] private float aimingSpeed = 4f;
    [SerializeField] protected float arrowSpeed = 20f;
    [SerializeField] private float turnSpeed = 720;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float interactRange = 1f;

    [SerializeField] protected Transform arrowSpawnPoint; 

    [SerializeField] protected ArrowBehaviour arrowPrefab;

    [SerializeField] private Gadget gadget;

    private float speed;
    protected float arrowCharge;
    protected float chargeStart = 0.8f;
    protected float chargeCap = 1.5f;
    protected int arrowCounter = 10;

    private bool movementEnabled, aimingEnabled, isReloading;
    protected bool fullCharge;

    private GameObject spawnPoint;

    private Rigidbody rb;

    protected ArrowBehaviour currentArrow;

    private Vector3 movementInput;
    private Vector3 aimDirection, mousePosition;

    private Camera mainCamera;

    private void Awake()
    {
        movementEnabled = false;
        aimingEnabled = false;
        fullCharge = false;
        spawnPoint = GameObject.FindWithTag("Respawn");
        rb = GetComponent<Rigidbody>();
        mainCamera = FindObjectOfType<Camera>();
        transform.position = new Vector3(spawnPoint.transform.position.x, 10, spawnPoint.transform.position.z);
        StartCoroutine(MovementEnabler());
    }

    private IEnumerator MovementEnabler()
    {
        yield return new WaitForSeconds(2);
        movementEnabled = true;
        aimingEnabled = true;
    }

    private void Spawn()
    {
        Reload();
        transform.position = spawnPoint.transform.position;
        movementEnabled = true;
        aimingEnabled = true;
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "ArrowPickUp")
        {
            PickUpArrow(1);
            Debug.Log("Arrow collected, current arrow count: " + arrowCounter);
            Destroy(coll.gameObject);
        }
        else if (coll.CompareTag("HealthPickUp"))
        {
            TakeDamage(-healthRegain);
            Debug.Log("Health collected, current health: " + health);
            Destroy(coll.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
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
            RotateLook();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ShootArrow();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gadget.GadgetAction();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (transform.position.y <= -3)
        {
            TakeDamage(fallDamage);
            Debug.Log("Current health: " + health);
            Spawn();
        }  
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(horizontal, 0, vertical);

        if (movementEnabled)
            rb.MovePosition(transform.position + movementInput * speed * Time.fixedDeltaTime);
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
        }
    }

    void RotateLook()
    {
        if (movementInput != Vector3.zero)
        {
            var rot = Quaternion.LookRotation(movementInput, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }
    }

    void Aiming()
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
        var force = transform.TransformDirection(Vector3.forward);
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint);
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.Shoot(transform.forward * finalArrowSpeed);
        //currentArrow.Shoot(transform.forward, finalArrowSpeed);
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

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void PickUpArrow(int arrows)
    {
        arrowCounter += arrows;
    }
}
