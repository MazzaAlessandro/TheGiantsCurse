using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCubeBehaviour : MonoBehaviour
{
    private bool isMelting;

    private Vector3 scaleChange = new Vector3(-0.01f, -0.01f, -0.01f);
    // Start is called before the first frame update
    void Awake()
    {
        isMelting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMelting)
        {
            transform.localScale += scaleChange;
            if (transform.localScale.x < 0.1f)
                Destroy(this.gameObject);
        }
    }

    public void StartMelting()
    {
        isMelting = true;
    }
}
