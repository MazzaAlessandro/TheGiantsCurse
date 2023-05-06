using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrelSpawn : MonoBehaviour
{
    [SerializeField] private GameObject explosiveBarrelPrefab;
    [SerializeField] private float spawnDelay = 5f;

    private bool canSpawn;

    private GameObject currentBarrel;

    private Vector3 spawnPoint;

    private void Awake()
    {
        currentBarrel = transform.GetChild(0).gameObject;
        canSpawn = true;
        spawnPoint = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
    }

    private void Update()
    {
        if ((transform.childCount == 0) && canSpawn && currentBarrel==null)
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
        currentBarrel = Instantiate(explosiveBarrelPrefab, transform);
        currentBarrel.transform.position = spawnPoint;
        canSpawn = true;
    }

}
