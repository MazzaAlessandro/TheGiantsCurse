using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillGadget : Gadget
{

    [SerializeField] private float cooldown = 11f;

    private bool canBeCanceled;

    // Start is called before the first frame update
    void Start()
    {
        isReady = true;
        canBeCanceled = false;
        gadgetUI.SetFillAmount(0);
        Debug.Log("Is Ready: " + isReady);
    }

    public override void GadgetAction()
    {
        Debug.Log("Is Ready: " + isReady);
        if (isReady)
        {
            gadgetUI.SetFillAmount(1);
            Debug.Log("Activate Drill Gadget! Cooldown: " + cooldown);
            isReady = false;
            canBeCanceled = true;
            GetComponentInParent<ChasmController>().Dig();
            StartCooldown(cooldown);
        }
        else if (canBeCanceled)
        {
            Debug.Log("cancel");
            GetComponentInParent<ChasmController>().ForcedEndDig();
            canBeCanceled = false;
        }
        else
            Debug.Log("Gadget on cooldown");
    }
}
