using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownPickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private float spawnDelay = 5f;

    private GameObject currentPickup;

    private bool canSpawn;

    private void Awake()
    {
        currentPickup = transform.GetChild(0).gameObject;
        canSpawn = true;
    }

    private void Update()
    {
        if ((transform.childCount == 0) && canSpawn && currentPickup == null)
        {
            canSpawn = false;
            StartCoroutine(SpawnDelay());
        }
    }

    private IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        Spawn();
    }

    private void Spawn()
    {
        currentPickup = Instantiate(pickupPrefab, transform);
        currentPickup.transform.position = this.transform.position;
        canSpawn = true;
    }
}
