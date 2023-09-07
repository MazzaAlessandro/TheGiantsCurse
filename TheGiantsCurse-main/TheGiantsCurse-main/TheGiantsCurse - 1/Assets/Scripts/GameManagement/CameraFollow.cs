using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraFollow : NetworkBehaviour
{
    private GameObject playerGameObject;

    private bool followGiant = false;

    private float yOffSet = 0f;
    private Vector3 vel;
    private Vector3 prevPos;

    //USe this one only in local testing
    private void Awake()
    {
        playerGameObject = GameObject.FindWithTag("Player");
        this.transform.SetParent(null);
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
        if (playerGameObject != null)
        {
            if (!followGiant)
            {
                transform.position = new Vector3(playerGameObject.transform.position.x, 0, playerGameObject.transform.position.z);
            }

            else
            {
                float vertical = Input.GetAxisRaw("Vertical");
                
                if (vertical < 0)
                    yOffSet = -2f;
                if (vertical > 0)
                    yOffSet = 10f;
                if (vertical == 0)
                    yOffSet = 0f;
                
                transform.position = Vector3.SmoothDamp(prevPos, new Vector3(playerGameObject.transform.position.x, yOffSet, playerGameObject.transform.position.z), ref vel, 0.5f);
                
                prevPos = transform.position;
            }
        }

             
    }

    public void ChangeFollow(GameObject newFocus)
    {
        playerGameObject = newFocus;
        transform.position = new Vector3(playerGameObject.transform.position.x, 0, playerGameObject.transform.position.z);
    }

    public void ChangeFollowGiant(GameObject newFocus)
    {
        playerGameObject = newFocus;
        followGiant = true;
        transform.position = new Vector3(playerGameObject.transform.position.x, 0, playerGameObject.transform.position.z);
    }
}
