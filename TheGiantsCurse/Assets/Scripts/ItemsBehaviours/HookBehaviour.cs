using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehaviour : MonoBehaviour
{

    private Rigidbody rb;
    private MoonriseController moonrise;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moonrise = GetComponentInParent<MoonriseController>();
    }

    public void Shoot(Vector3 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grapple") || collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hook hit a grapple point");
            moonrise.PullTowards(collision.transform.position);
        }
        moonrise.SetMovement(true);
        Destroy(this.gameObject);
    }
}
