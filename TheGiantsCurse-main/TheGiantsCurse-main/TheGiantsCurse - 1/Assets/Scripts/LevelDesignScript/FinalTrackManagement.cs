using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This should be shared between all players, one single FinalTrackManagement in all the lobby
//Either that or this needs to basically be redone
public class FinalTrackManagement: MonoBehaviour
{
    public static FinalTrackManagement instance;

    [SerializeField] private GameObject[] playersSpawnPoints;
    [SerializeField] public GameObject giantSpawnpoint;

    public List<GameObject> players;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        for(int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<PlayerController>().SetSpawnpoint(playersSpawnPoints[i]);
        }
    }

    public void PlayerDied(GameObject player)
    {
        players.Remove(player);

        if(players.Count == 0)
        {
            Debug.Log("Everyone died, the giant should win");
            //LevelManager.instance.GiantVictory();
        }
    }

    public void PlayerEscaped(GameObject escapedPlayer)
    {
        Debug.Log("Someone escaped");
        foreach(GameObject player in players)
        {
            if (!player.Equals(escapedPlayer))
            {
                player.GetComponent<PlayerController>().Defeat();
            }
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
