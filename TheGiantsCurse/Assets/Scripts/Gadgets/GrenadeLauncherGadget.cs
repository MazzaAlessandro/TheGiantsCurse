using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherGadget : Gadget
{
    [SerializeField] private Explosive grenadePrefab;
    [SerializeField] private float grenadeSpeed = 15f;
    [SerializeField] private float cooldown = 6f;

    private Explosive grenade;

    private float countdown;

    private bool isReady;
    // Start is called before the first frame update
    void Start()
    {
        countdown = 0f;
        isReady = true;
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
    }

    public override void GadgetAction()
    {
        if (isReady)
        {
            ShootGrenade();
        }
        else
            Debug.Log("Gadget on cooldown");
    }

    private void ShootGrenade()
    {
        grenade = Instantiate(grenadePrefab, transform);
        grenade.transform.localPosition = Vector3.forward;
        grenade.transform.SetParent(null);
        grenade.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * grenadeSpeed, ForceMode.Impulse);
        grenade = null;
        Debug.Log("Activate Grenade Launcher Gadget! Cooldown: " + cooldown);
        countdown = 0f;
        isReady = false;
    }
}
