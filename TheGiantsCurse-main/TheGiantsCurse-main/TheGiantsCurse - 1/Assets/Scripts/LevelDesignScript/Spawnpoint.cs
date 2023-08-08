using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public bool first;

    private void Awake()
    {
        /*if(first)
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SetSpawnpoint(this.gameObject);*/
    }
}
