using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraFollow : NetworkBehaviour
{
    private GameObject playerGameObject;

    //USe this one only in local testing
    private void Awake()
    {
        playerGameObject = GameObject.FindWithTag("Player");
    }

    // This version should correctly handle the camera follow in the NetCode Envrionment
    /*void Start()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlayerController>().IsOwner)
                playerGameObject = player;
        }
        
    }*/

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) 
            enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerGameObject!=null)
            transform.position = new Vector3 (playerGameObject.transform.position.x, 0, playerGameObject.transform.position.z); 
    }

    public void ChangeFollow(GameObject newFocus)
    {
        playerGameObject = newFocus;
    }
}
