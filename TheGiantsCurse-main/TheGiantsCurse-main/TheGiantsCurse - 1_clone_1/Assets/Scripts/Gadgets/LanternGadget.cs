using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternGadget : Gadget
{
    [SerializeField] private float cooldown = 5f;

    [SerializeField] private float lanternIntensity = 5f;

    [SerializeField] private Light lanternLight;
    [SerializeField] private GameObject firePrefab;

    private Light globalLight;

    private GameObject fire;

    private bool lightOn;
    // Start is called before the first frame update
    void Start()
    {
        //globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light>();
        isReady = true;
        /*if(globalLight.intensity < 0.5f)
        {
            lanternLight.intensity = lanternIntensity;
            lightOn = true;
        }
        else
        {
            lanternLight.intensity = 0;
            lightOn = false;
        }*/
        lanternLight.intensity = 0;
        gadgetUI.SetFillAmount(0);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (globalLight == null && GameObject.FindWithTag("GlobalLight").GetComponent<Light>() != null)
            globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light>();

        if (globalLight != null)
        {
            if (globalLight.intensity < 0.5f && !lightOn)
            {
                lightOn = true;
                StartCoroutine(Helper.FadeLight(lanternLight, 0f, 5f, 1f));
            }
            else if (globalLight.intensity >= 0.5f && lightOn)
            {
                lightOn = false;
                StartCoroutine(Helper.FadeLight(lanternLight, 5f, 0f, 1f));
            }
        }

        base.Update();
    }

    public override void GadgetAction()
    {
        if (isReady)
        {
            CreateFire();
        }
        else
            Debug.Log("Gadget on cooldown");
    }

    private void CreateFire()
    {
        gadgetUI.SetFillAmount(1);
        fire = Instantiate(firePrefab, transform);
        fire.transform.localPosition = Vector3.forward * 1.5f;
        fire.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 8, ForceMode.Impulse);
        fire.transform.SetParent(null);
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(1f);
        Destroy(fire);
        fire = null;
        isReady = false;
        StartCooldown(cooldown);
    }

    public void SetGlobalLight(Light gl)
    {
        globalLight = gl;
    }
}
