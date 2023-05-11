using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardEvent : MonoBehaviour
{
    public static HazardEvent instance;

    [SerializeField] private float darknessDuration = 5f;

    private Light globalLight;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light>();
    }

    void Darkness(float duration)
    {

    }

}
