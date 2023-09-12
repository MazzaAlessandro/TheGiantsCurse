using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessTrigger : MonoBehaviour
{
    [SerializeField] private bool LightToDark;

    private Light globalLight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PlayerController>().enabled)
            {
                globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light>();

                if (LightToDark)
                {
                    if (globalLight.intensity == 1)
                        StartCoroutine(Helper.FadeLight(globalLight, 1, 0, 1f));

                }
                else
                {
                    if (globalLight.intensity == 0)
                        StartCoroutine(Helper.FadeLight(globalLight, 0, 1, 1f));
                }
            }
            
        }
    }
}
