using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private List<Transform> spawnpoints;
    [SerializeField] private CharacterDatabase characterDatabase;

    private Vector3 offset = new Vector3(0, 10, 0);
    private int spawnAvailable = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (var client in ServerManager.instance.clientData)
        {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if(character!= null)
            {
                Debug.Log("Should spawn character: " + client.Value.characterId + "at position: " + spawnAvailable);

                var charInstance = Instantiate(character.GameplayPrefab, spawnpoints[spawnAvailable].position + offset, Quaternion.identity);
                charInstance.SpawnAsPlayerObject(client.Value.clientId);

                charInstance.gameObject.GetComponent<PlayerController>().SetPlayerCode(spawnAvailable);
                spawnAvailable++;
            }
        }
    }
}
