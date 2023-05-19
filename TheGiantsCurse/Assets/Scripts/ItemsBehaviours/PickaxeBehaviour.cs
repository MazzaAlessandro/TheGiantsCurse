using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject pickaxePickupPrefab;
    [SerializeField] private int pickaxeDamage;

    private GameObject pickup;
    private bool thrown = false;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Destroyable":
                if (thrown)
                {
                    other.gameObject.GetComponent<DestroyableObject>().ObstacleDestruction();
                    SpawnPickUp();
                }
                else
                    other.gameObject.GetComponent<DestroyableObject>().TakeDamage(pickaxeDamage);
                break;
            case "Player":
                //damages the player, is not destroyed but passes through it
                break;
            default:
                if (thrown)
                    SpawnPickUp();
                break;
        }
    }

    private void SpawnPickUp()
    {
        transform.eulerAngles = new Vector3(0,0,0);
        pickup = Instantiate(pickaxePickupPrefab, transform);
        pickup.transform.localPosition = new Vector3(0, 0, -1);
        pickup.transform.SetParent(null);
        pickup.transform.localScale = Vector3.one;
        Destroy(this.gameObject);
    }

    public void SetThrown()
    {
        thrown = true;
    }
}
