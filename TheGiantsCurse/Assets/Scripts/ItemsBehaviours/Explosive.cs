using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float explosionRange = 3f;
    [SerializeField] private float playerDamage = 5f;
    [SerializeField] private float explosionDelay = 3f;

    [SerializeField] private GameObject explosionEffect;
    private GameObject explosionInstance;

    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip tickingSound;
    [SerializeField] private AudioClip fallSound;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    private void Update()
    {
        if (transform.position.y <= -2)
        {
            SoundManager.instance.PlayEffect(fallSound);
            Destroy(this.gameObject);
        }
    }
    public void Ignite()
    {
        Debug.Log("The barrel will explode in: " + explosionDelay);
        SoundManager.instance.PlayMultipleTimes(tickingSound, tickingSound.length, 3);
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    public void Explode()
    {
        float explosionScale = explosionRange * 0.7f;

        explosionInstance = Instantiate(explosionEffect, transform.position, transform.rotation);
        explosionInstance.transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);

        SoundManager.instance.PlayEffect(explosionSound);

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
                objectHit.GetComponent<PlayerController>().Stun(0.5f);
                objectHit.GetComponent<Rigidbody>().AddExplosionForce(5, transform.position, explosionRange);
            }

            if (objectHit.CompareTag("Explosive") && objectHit.gameObject!=this.gameObject)
            {
                objectHit.GetComponent<Explosive>().Ignite();
            }
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Destroyable") || other.gameObject.CompareTag("IceBlock") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Explosive"))
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
