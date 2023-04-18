using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float turnSpeed = 720;

    private Vector3 movementInput;
    
    
    // Update is called once per frame
    void Update()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        RotateLook();
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + movementInput.ToIso() * movementSpeed * Time.fixedDeltaTime);
    }

    void RotateLook()
    {
        if (movementInput != Vector3.zero)
        {
            var rot = Quaternion.LookRotation(movementInput.ToIso(), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }
    }
}
