using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float aimingSpeed = 3f;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private float turnSpeed = 720;

    private float speed;
    private float arrowCharge;
    private float chargeStart = 0.5f;
    private float chargeCap = 2f;
    
    private Rigidbody rb;
    private Vector3 movementInput;
    private Vector3 aimDirection, mousePosition;

    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
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
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(horizontal, 0, vertical);

        rb.MovePosition(transform.position + movementInput.ToIso() * speed * Time.fixedDeltaTime);        
    }

    void RotateLook()
    {
        if (movementInput != Vector3.zero)
        {
            var rot = Quaternion.LookRotation(movementInput.ToIso(), Vector3.up);
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
        float finalArrowSpeed = arrowSpeed * arrowCharge;
        Debug.Log("Arrow Speed is: " + finalArrowSpeed);
        arrowCharge = chargeStart;
    }
}
