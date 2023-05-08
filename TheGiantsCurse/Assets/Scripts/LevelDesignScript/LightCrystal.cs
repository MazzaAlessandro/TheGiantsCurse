using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCrystal : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -3)
        {
            Destroy(this.gameObject);
        }
    }
}
