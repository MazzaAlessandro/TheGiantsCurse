using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType { ARROW, HEALTH, ROPE}

public class AttractorBehaviour : MonoBehaviour
{
    [SerializeField] private float attractionSpeed = 10f;
    [SerializeField] private PickUpType type;

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount < 1)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(type.Equals(PickUpType.HEALTH) && other.GetComponent<PlayerController>().AtMaxHealth())
            {
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, other.transform.position, attractionSpeed * Time.deltaTime);
        }
    }
}
