using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
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
    }
}
