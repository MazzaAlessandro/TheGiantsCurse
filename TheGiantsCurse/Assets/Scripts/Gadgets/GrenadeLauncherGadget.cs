using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherGadget : Gadget
{
    [SerializeField] private Explosive grenadePrefab;
    [SerializeField] private float grenadeSpeed = 15f;
    [SerializeField] private float cooldown = 6f;

    [SerializeField] private AudioClip throwSound;

    private Explosive grenade;

    // Start is called before the first frame update
    void Start()
    {
        isReady = true;
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
        SoundManager.instance.PlayEffect(throwSound);
        grenade = null;
        Debug.Log("Activate Grenade Launcher Gadget! Cooldown: " + cooldown);
        isReady = false;
        StartCooldown(cooldown);
    }
}
