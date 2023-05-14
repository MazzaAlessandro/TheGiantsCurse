using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float explosionRange = 3f;
    [SerializeField] private float playerDamage = 5f;
    [SerializeField] private float explosionDelay = 3f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    private void Update()
    {
        if (transform.position.y <= -3)
        {
            Destroy(this.gameObject);
        }
    }
    public void Ignite()
    {
        Debug.Log("The barrel will explode in: " + explosionDelay);
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    public void Explode()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionRange);
        foreach(var objectHit in objectsInRange)
        {
            if (objectHit.CompareTag("Destroyable"))
            {
                objectHit.GetComponent<DestroyableObject>().ObstacleDestruction();
            }
            if (objectHit.CompareTag("IceBlock"))
            {
                objectHit.GetComponent<IceCubeBehaviour>().StartMelting();
            }
            if (objectHit.CompareTag("Player"))
            {
                objectHit.GetComponent<PlayerController>().TakeDamage(playerDamage);
            }
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Destroyable") || other.gameObject.CompareTag("IceBlock"))
        {
            Debug.Log("HIT SOMETHING, I'M GONNA EXPLODE NOW");
            Explode();
        }
    }

    public void MakeTrigger()
    {
        this.gameObject.GetComponent<Collider>().isTrigger = true;
    }
}
