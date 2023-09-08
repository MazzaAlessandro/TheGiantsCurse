using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelGenerator : NetworkBehaviour
{
    public static LevelGenerator instance;

    private List<int> tmpRoomsSet = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
    private List<int> roomsSequence;
 
    [SerializeField] private List<GameObject> roomsPrefabs;
    [SerializeField] private List<Transform> roomsLocations;

    //These are needed to handle the assignment of which exit takes to which room
    [SerializeField] private GameObject initialExit;
    private GameObject roomInstance;
    private GameObject prevRoomExit;
    private GameObject nextSpawnPoint;

    private string seed = "";

    //This is a LOCAL version of the process. In the final prototype, in Awake only the server starts the RoomSetup
    //At the end of RoomSetup, the server sends the seed to all clients with an rcp
    //All clients generate the rooms with SeedParser

    void Awake()
    {
        //Local test
        //RoomsSetup(5); 
        //RoomsSetup(ServerManager.instance.clientData.Count);
        if(instance!=null && instance != this)
        {
            Destroy(this);
        }
        else if (instance == null)
        {
            instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("I'm Server");
            RoomsSetup(ServerManager.instance.clientData.Count);
        }

        else
        {
            Debug.Log("I'm not");
            SeedRequest(NetworkManager.LocalClientId);
        }
    }

    private void RoomsSetup(int players)
    {
        int cursor;
        int numOfPlayers = players;

        for (int j = 0; j < numOfPlayers; j++)
        {
            roomsSequence = new List<int>();
            tmpRoomsSet = new List<int> { 0, 1, 2, 3, 4, 5, 6 };

            for (int i = 0; i < 4; i++)
            {
                cursor = UnityEngine.Random.Range(0, tmpRoomsSet.Count);
                roomsSequence.Add(tmpRoomsSet[cursor]);
                tmpRoomsSet.RemoveAt(cursor);
                
                seed += roomsSequence[i];
                if (i < 3)
                    seed += ".";
            }

            if(j < numOfPlayers - 1)
                seed += "|";

        }

        Debug.Log(seed);

        SeedParser(seed);
    }

    private void SeedParser(string seed)
    {
        string[] columns = seed.Split("|");
        string[] rooms;

        int tmp;

        for(int j = 0; j < columns.Length; j++)
        {
            Debug.Log(columns[j]);

            rooms = columns[j].Split(".");
            for (int i = 0; i < rooms.Length; i++)
            {
                Debug.Log(rooms[i]);

                tmp = Int32.Parse(rooms[i]);

                roomInstance = Instantiate(roomsPrefabs[tmp], roomsLocations[i + 4 * j]);
                nextSpawnPoint = Helper.FindGameObjectInChildWithTag(roomInstance, "Respawn");

                if (i == 0)
                {
                    initialExit.GetComponent<FirstExit>().nextRoomSpawnpoints.Add(nextSpawnPoint);
                }
                else
                {
                    prevRoomExit.GetComponent<ExitLevel>().SetNextRoomSpawn(nextSpawnPoint);
                }
                prevRoomExit = Helper.FindGameObjectInChildWithTag(roomInstance, "Exit");
                if (i == 3)
                    prevRoomExit.GetComponent<ExitLevel>().MakeFinalRoom();
            }
        }

    }

    public void SeedRequest(ulong client)
    {
        Debug.Log("Get request");
        SeedRequestServerRpc(client);
    }

    [ServerRpc(RequireOwnership =false)]
    private void SeedRequestServerRpc(ulong client)
    {
        if (!IsServer) 
            return;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { client }
            }
        };

        SeedRequestClientRpc(seed, clientRpcParams);
    }

    [ClientRpc]
    private void SeedRequestClientRpc(string seedRecieved, ClientRpcParams clientRpcParams = default)
    {
        seed = seedRecieved;
        SeedParser(seed);
    }
}
