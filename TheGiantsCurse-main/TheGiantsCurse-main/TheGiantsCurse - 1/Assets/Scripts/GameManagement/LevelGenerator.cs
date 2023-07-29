using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private List<int> tmpRoomsSet = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
    private List<int> roomsSequence;

    //public List<List<Transform>> roomsPositions = new List<List<Transform>>(); 
    [SerializeField] private List<GameObject> roomsPrefabs;
    [SerializeField] private List<Transform> roomsLocations;

    //These are needed to handle the assignment of which exit takes to which room
    [SerializeField] private GameObject initialExit;
    private GameObject roomInstance;
    private GameObject prevRoomExit;
    private GameObject nextSpawnPoint;

    private string seed = "";
    // Start is called before the first frame update
    void Awake()
    {
        RoomsSetup(5);
    }

    private void RoomsSetup(int players)
    {
        int cursor;
        int numOfPlayers = players;

        for (int j = 0; j < numOfPlayers; j++)
        {
            roomsSequence = new List<int>();
            tmpRoomsSet = new List<int> { 0, 1, 2, 3, 4, 5, 6 };

            seed += "|";

            for (int i = 0; i < 4; i++)
            {
                cursor = UnityEngine.Random.Range(0, tmpRoomsSet.Count);
                roomsSequence.Add(tmpRoomsSet[cursor]);
                tmpRoomsSet.RemoveAt(cursor);
                if (i == 0)
                    prevRoomExit = initialExit;
                roomInstance = Instantiate(roomsPrefabs[roomsSequence[i]], roomsLocations[i + 4 * j]);
                nextSpawnPoint = Helper.FindGameObjectInChildWithTag(roomInstance, "Respawn");
                prevRoomExit.GetComponent<ExitLevel>().SetNextRoomSpawn(nextSpawnPoint);
                prevRoomExit = Helper.FindGameObjectInChildWithTag(roomInstance, "Exit");
                if (i == 3)
                    prevRoomExit.GetComponent<ExitLevel>().MakeFinalRoom();
                seed += roomsSequence[i];
            }
        }

        Debug.Log(seed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
