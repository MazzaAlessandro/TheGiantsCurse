using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternGadget : Gadget
{
    [SerializeField] private float cooldown = 3f;

    [SerializeField] private float lanternIntensity = 5f;

    [SerializeField] private Light lanternLight;

    private Light globalLight;

    private float countdown;

    private bool isReady;
    private bool lightOn;
    // Start is called before the first frame update
    void Start()
    {
        globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light>();
        lanternLight.gameObject.SetActive(true);
        countdown = 0f;
        isReady = true;
        lightOn = (globalLight.intensity < 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
        {
            countdown += Time.deltaTime;
            if (countdown >= cooldown)
            {
                isReady = true;
                Debug.Log("Gadget is ready!");
            }
        }

        //StartCoroutine(LanternFade(globalLight.intensity < 0.5f));
        if (globalLight.intensity < 0.5f && !lightOn)
        {
            //lanternLight.gameObject.SetActive(true);
            lightOn = true;
            StartCoroutine(Helper.FadeLight(lanternLight, 0f, 5f, 1f));
        }
        else if (globalLight.intensity >= 0.5f && lightOn)
        {
            lightOn = false;
            StartCoroutine(Helper.FadeLight(lanternLight, 5f, 0f, 1f));
            //lanternLight.gameObject.SetActive(false);
        }

    }


    private IEnumerator LanternFade(bool on)
    {
        float interval = 0.1f;
        if (on)
        {
            while (lanternLight.intensity <= lanternIntensity)
            {
                lanternLight.intensity += 0.02f;
                yield return new WaitForSeconds(interval);
            }
        }
        else
        {
            while (lanternLight.intensity >= 0)
            {
                lanternLight.intensity -= 0.02f;
                yield return new WaitForSeconds(interval);
            }
        }
    }

    public override void GadgetAction()
    {
        if (isReady)
        {
            Debug.Log("Activate Lantern Gadget! Cooldown: " + cooldown);
            countdown = 0f;
            isReady = false;
        }
        else
            Debug.Log("Gadget on cooldown");
    }
}
