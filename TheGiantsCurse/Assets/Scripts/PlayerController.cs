using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    [SerializeField] private float fallDamage = 1f;
    [SerializeField] private float healthRegain = 5f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float aimingSpeed = 3f;
    [SerializeField] private float arrowSpeed = 20f;
    [SerializeField] private float turnSpeed = 720;
    [SerializeField] private float reloadTime = 1f;

    [SerializeField] private Transform arrowSpawnPoint;

    [SerializeField] private ArrowBehaviour arrowPrefab;

    private float speed;
    private float arrowCharge;
    private float chargeStart = 0.8f;
    private float chargeCap = 1.5f;
    private int arrowCounter = 10;

    private bool movementEnabled, aimingEnabled, isReloading;

    private GameObject spawnPoint;

    private Rigidbody rb;

    private ArrowBehaviour currentArrow;

    private Vector3 movementInput;
    private Vector3 aimDirection, mousePosition;

    private Camera mainCamera;

    private void Awake()
    {
        spawnPoint = GameObject.FindWithTag("Respawn");
        rb = GetComponent<Rigidbody>();
        mainCamera = FindObjectOfType<Camera>();
        Spawn();
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
            arrowCounter++;
            Debug.Log("Arrow collected, current arrow count: " + arrowCounter);
            Destroy(coll.gameObject);
        }
        else if (coll.CompareTag("HealthPickUp"))
        {
            health += healthRegain;
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

        if (transform.position.y <= -3)
        {
            health -= fallDamage;
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
        arrowCharge += Time.deltaTime;
        if (arrowCharge >= chargeCap)
        {
            arrowCharge = chargeCap;
            Debug.Log("Maximum charge reached at: " + arrowCharge);
        }
            

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

    //right now this does not shoot anything, I'm just checking the math
    void ShootArrow()
    {
        if (isReloading)
        {
            Debug.Log("You are reloading");
            return; 
        }
        if (arrowCounter >= 1)
        {
            float finalArrowSpeed = arrowSpeed * arrowCharge;
            arrowCharge = chargeStart;
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
        else
        {
            Debug.Log("Out of arrows");
        }
    }

    void Reload()
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
}
