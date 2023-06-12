using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    private void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SetSpawnpoint(this.gameObject);
    }
}
