using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class AreaOfCollision : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Now player dies");
            other.gameObject.GetComponent<PlayerNetworkActions>().TakeDamage(999f);
        }
    }
}
