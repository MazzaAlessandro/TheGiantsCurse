using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonriseController : PlayerController
{
    public override void TakeDamage(float damage)
    {
        if (!grappled) {
            base.TakeDamage(damage);
        }
    }

    public override void PullTowards(Vector3 destination)
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

    public override void GrappledMovement()
    {
        if (Vector3.Distance(transform.position, movementInput) < 2f)
        {
            grappled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            lineRenderer.enabled = false;
            SetMovement(true);
        }
        else
        {
            rb.AddForce((movementInput - transform.position).normalized, ForceMode.VelocityChange);
        }
    }

    public void SetMovement(bool active)
    {
        movementEnabled = active;
        aimingEnabled = active;
    }
}
