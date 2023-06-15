using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
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
