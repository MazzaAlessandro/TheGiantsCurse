using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject playerGameObject; 

    // Start is called before the first frame update
    void Awake()
    {
        playerGameObject = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3 (playerGameObject.transform.position.x, 0, playerGameObject.transform.position.z); 
    }
}
