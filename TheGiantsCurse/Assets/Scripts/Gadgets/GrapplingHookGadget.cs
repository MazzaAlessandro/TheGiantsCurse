using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookGadget : Gadget
{
    [SerializeField] private float hookSpeed = 10f;
    [SerializeField] private float cooldown = 3f;

    [SerializeField] private HookBehaviour Hook;

    private HookBehaviour hookInstance;

    // Start is called before the first frame update
    void Start()
    {
        gadgetUI.SetFillAmount(0);
        isReady = true;
    }


    public override void GadgetAction()
    {
        if (isReady)
        {
            GetComponentInParent<MoonriseController>().SetMovement(false);
            Debug.Log("Activate Grappling Hook Gadget! Cooldown: " + cooldown);
            hookInstance = Instantiate(Hook, transform);
            hookInstance.transform.localPosition = Vector3.forward;
            hookInstance.Shoot(transform.forward * hookSpeed);
            hookInstance = null;
            isReady = false;
            StartCooldown(cooldown);
        }
        else
            Debug.Log("Gadget on cooldown");
    }
}
