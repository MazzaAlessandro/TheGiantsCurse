using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject arrowPickupPrefab;

    private Rigidbody rb;
    private GameObject arrowPickup;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -3)
        {
            Destroy(this.gameObject);
        }
    }

    public void Shoot(Vector3 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
        transform.SetParent(null);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Wall"))
        {
            Debug.Log("Arrow hit a wall!");
            SpawnAmmoPickup();
        }
    }

    private void SpawnAmmoPickup()
    {
        arrowPickup = Instantiate(arrowPickupPrefab, transform);
        arrowPickup.transform.localPosition = new Vector3(0, 0, -1);
        arrowPickup.transform.SetParent(null);
        arrowPickup.transform.localScale = Vector3.one;
        Destroy(this.gameObject);
    }
}
