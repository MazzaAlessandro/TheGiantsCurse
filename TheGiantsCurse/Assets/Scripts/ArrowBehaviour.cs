using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    private bool collectable = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool IsCollectable()
    {
        return collectable;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -3)
        {
            Destroy(this.gameObject);
        }
    }
}
