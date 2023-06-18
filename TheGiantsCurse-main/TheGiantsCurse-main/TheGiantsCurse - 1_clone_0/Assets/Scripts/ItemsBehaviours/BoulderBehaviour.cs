using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehaviour : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(5);
            collision.gameObject.GetComponent<PlayerController>().Stun(1.2f);
        }

        else if (collision.gameObject.CompareTag("Destroyable"))
        {
            collision.gameObject.GetComponent<DestroyableObject>().ObstacleDestruction();
        }

        else if (collision.gameObject.CompareTag("Explosive"))
        {
            collision.gameObject.GetComponent<Explosive>().Explode();
        }

        else if (collision.gameObject.CompareTag("Grapple") || collision.gameObject.CompareTag("IceBlock") || collision.gameObject.CompareTag("Torch"))
        {
            Destroy(collision.gameObject);
        }
    }
}
