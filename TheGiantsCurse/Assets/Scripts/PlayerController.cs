using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float movementSpeed = 5f;

    private Vector3 movementInput;
    
    // Update is called once per frame
    void Update()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + movementInput * movementSpeed * Time.deltaTime);
    }
}
