using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTrackSpawnPoints : MonoBehaviour
{
    [SerializeField] private GameObject[] playersSpawnPoints;
    [SerializeField] private GameObject giantSpawnpoint;

    private GameObject[] players;

    // Start is called before the first frame update
    void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++)
        {
            players[0].GetComponent<PlayerController>().SetSpawnpoint(playersSpawnPoints[0]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Assign a spawnpoint to one of the non-giant players
    public void AssignSpawn(PlayerController player, int spawn)
    {
        player.SetSpawnpoint(playersSpawnPoints[spawn]);
    }
}
