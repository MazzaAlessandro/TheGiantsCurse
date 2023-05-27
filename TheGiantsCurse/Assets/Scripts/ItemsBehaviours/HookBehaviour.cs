using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehaviour : MonoBehaviour
{
    private bool shoot;

    private Rigidbody rb;
    private MoonriseController moonrise;
    private LineRenderer lineRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moonrise = GetComponentInParent<MoonriseController>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (shoot)
        {
            Vector3[] positions = new Vector3[]
            {
                moonrise.transform.position,
                transform.position
            };

            lineRenderer.SetPositions(positions);
        }
    }

    public void Shoot(Vector3 force)
    {
        shoot = true;
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
