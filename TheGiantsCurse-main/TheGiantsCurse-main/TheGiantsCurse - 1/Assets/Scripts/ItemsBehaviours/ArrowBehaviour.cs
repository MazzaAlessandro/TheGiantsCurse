using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    [SerializeField] private float damage = 5f;

    [SerializeField] private GameObject arrowPickupPrefab;
    [SerializeField] private GameObject tipLight;

    [SerializeField] private AudioClip fireSound;

    private Rigidbody rb;
    private GameObject arrowPickup;
    private GameObject owner;
    private LineRenderer lineRenderer;

    private bool fireArrow = false;
    private bool ropedArrow = false;

    // Start is called before the first frame update
    void Awake()
    {
        tipLight.SetActive(false);
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ropedArrow)
        {
            Vector3[] positions = new Vector3[]
            {
                owner.transform.position,
                transform.position
            };

            lineRenderer.SetPositions(positions);
        }

        if (transform.position.y <= -3)
        {
            Destroy(this.gameObject);
        }
    }

    public void Shoot(Vector3 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
        if (!ropedArrow)
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
            case "Destroyable":
                if (ropedArrow)
                {
                    GetComponentInParent<PlayerController>().PullTowards(coll.transform.position);
                }
                SpawnAmmoPickup();
                break;
            case "Pickup":
                SpawnAmmoPickup();
                break;
            case "Switch":
                Debug.Log("Arrow hit a switch");
                SpawnAmmoPickup();
                coll.GetComponent<SwitchAction>().SetActive();
                break;
            //this was only to test the timer on switches. They should be interactable, not to hit with arrows
            /*case "TimedSwitch":
                Debug.Log("Arrow hit a timed switch");
                SpawnAmmoPickup();
                coll.GetComponent<TimedSwitchAction>().SetActive();
                break;*/
            case "Torch":
                Debug.Log("Arrow hit a torch");
                if (coll.GetComponent<TorchBehaviour>().IsLit() && !fireArrow)
                {
                    MakeFireArrow();
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
            case "Grapple":
                Debug.Log("Hit a grappling point");
                if (ropedArrow)
                {
                    GetComponentInParent<PlayerController>().PullTowards(coll.transform.position);
                }
                SpawnAmmoPickup();
                break;
            case "Explosive":
                Debug.Log("Explosive barrel was hit");
                if (fireArrow)
                    coll.GetComponent<Explosive>().Explode();
                else
                    coll.GetComponent<Explosive>().Ignite();
                SpawnAmmoPickup();
                break;
            case "Player":
                if (coll.gameObject != owner)
                {
                    Debug.Log("Hit a player");
                    coll.GetComponent<PlayerController>().Stun(1.2f);
                    if (fireArrow)
                        coll.GetComponent<PlayerController>().TakeFireDamage();
                    else
                        coll.GetComponent<PlayerController>().TakeDamage(damage);
                }
                break;
        }
    }

    private void SpawnAmmoPickup()
    {
        arrowPickup = Instantiate(arrowPickupPrefab, transform);
        arrowPickup.transform.localPosition = new Vector3(0, 0, -1);
        if (ropedArrow)
            owner.GetComponent<PlayerController>().EnableMovement();
        arrowPickup.transform.SetParent(null);
        arrowPickup.transform.localScale = new Vector3(1, 1, 0.25f);
        Destroy(this.gameObject);
    }

    public void MakeFireArrow()
    {
        tipLight.SetActive(true);
        SoundManager.instance.PlayEffect(fireSound);
        fireArrow = true;
    }
    public void MakeRoped()
    {
        ropedArrow = true;
    }

    public void SetOwner(GameObject own)
    {
        owner = own;
    }
}
