using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject arrowPickupPrefab;

    private Rigidbody rb;
    private GameObject arrowPickup;

    private bool fireArrow = false;

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
        switch (coll.tag)
        {
            case "Wall":
                Debug.Log("Arrow hit a wall!");
                SpawnAmmoPickup();
                break;
            case "Switch":
                Debug.Log("Arrow hit a switch");
                SpawnAmmoPickup();
                coll.GetComponent<SwitchAction>().SetActive();
                break;
            case "Torch":
                Debug.Log("Arrow hit a torch");
                if (coll.GetComponent<TorchBehaviour>().IsLit() && !fireArrow)
                {
                    fireArrow = true;
                    Debug.Log("Arrow is now on fire");
                }
                if (!coll.GetComponent<TorchBehaviour>().IsLit() && fireArrow)
                {
                    coll.GetComponent<TorchBehaviour>().LitTorch();
                }
                break;
            case "IceBlock":
                Debug.Log("Arrow hit an ice block");
                if (fireArrow)
                {
                    Debug.Log("The ice block melts");
                    coll.GetComponent<IceCubeBehaviour>().StartMelting();
                }
                SpawnAmmoPickup();
                break;
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
