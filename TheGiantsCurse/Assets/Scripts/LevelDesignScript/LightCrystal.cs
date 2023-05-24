using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCrystal : MonoBehaviour
{
    [SerializeField] private float crystalIntensity = 4f;
    [SerializeField] private AudioClip fallSound;

    private Light crystalLight;
    private Light globalLight;

    private bool lightOn;

    private void Start()
    {
        crystalLight = GetComponentInChildren<Light>();
        globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light>();
        if (globalLight.intensity < 0.5f)
        {
            crystalLight.intensity = crystalIntensity;
            lightOn = true;
        }
        else
        {
            crystalLight.intensity = 0;
            lightOn = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (globalLight != null)
        {
            if (globalLight.intensity < 0.5f && !lightOn)
            {
                lightOn = true;
                StartCoroutine(Helper.FadeLight(crystalLight, 0f, crystalIntensity, 1f));
            }
            else if (globalLight.intensity >= 0.5f && lightOn)
            {
                lightOn = false;
                StartCoroutine(Helper.FadeLight(crystalLight, crystalIntensity, 0f, 1f));
            }
        }

        if (transform.position.y < -3)
        {
            SoundManager.instance.PlayEffect(fallSound);
            Destroy(this.gameObject);
        }
    }
}
