using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject spawnPoint;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float aimingSpeed = 3f;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private float turnSpeed = 720;

    private float speed;
    private float arrowCharge;
    private float chargeStart = 0.5f;
    private float chargeCap = 2f;
    private int arrowCounter = 10;
    private int maxArrowCapacity = 10;

    private bool movementEnabled, aimingEnabled;
    
    private Rigidbody rb;

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
        transform.position = spawnPoint.transform.position;
        movementEnabled = true;
        aimingEnabled = true;
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Arrow" && arrowCounter < maxArrowCapacity)
        {
            arrowCounter++;
            Debug.Log("Arrow collected, current arrow count: " + arrowCounter);
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
            Spawn();
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
        if (arrowCounter > 0)
        {
            float finalArrowSpeed = arrowSpeed * arrowCharge;
            arrowCharge = chargeStart;
            arrowCounter--;
            Debug.Log("Arrow Speed is: " + finalArrowSpeed + " and remaining arrows are: " + arrowCounter);
        } 
        else
        {
            Debug.Log("Out of arrows");
        }
    }
}
