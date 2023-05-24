using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    private GameObject owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject!=owner)
                other.gameObject.GetComponent<PlayerController>().TakeFireDamage();
        }
        if (other.gameObject.CompareTag("Torch"))
        {
            Debug.Log("hit a torch!");
            if (!other.GetComponent<TorchBehaviour>().IsLit())
                other.GetComponent<TorchBehaviour>().LitTorch();
        }
        if (other.gameObject.CompareTag("IceBlock"))
        {
            Debug.Log("hit an ice cube");
            other.GetComponent<IceCubeBehaviour>().StartMelting();
        }
        if (other.gameObject.CompareTag("Explosive"))
        {
            other.GetComponent<Explosive>().Explode();
        }
    }

    public void SetOwner(GameObject own)
    {
        owner = own;
    }
}
