using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float aimingSpeed = 2.5f;
    [SerializeField] private float turnSpeed = 720;

    private float speed;
    private Rigidbody rb;
    private Vector3 movementInput;
    private Vector3 AimDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(horizontal, 0, vertical);

        if (Input.GetMouseButton(1))
            speed = aimingSpeed;
        else speed = movementSpeed;
        rb.MovePosition(transform.position + movementInput.ToIso() * speed * Time.fixedDeltaTime);
        RotateLook();
    }

    void RotateLook()
    {
        if (Input.GetMouseButton(1))
        {
            //turn towards mouse direction

        }
        else if (movementInput != Vector3.zero)
        {
            var rot = Quaternion.LookRotation(movementInput.ToIso(), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }
    }
}
