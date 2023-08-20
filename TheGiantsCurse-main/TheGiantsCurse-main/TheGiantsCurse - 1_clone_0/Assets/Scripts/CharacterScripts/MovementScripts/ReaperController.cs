using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperController : PlayerController
{
    protected bool hasPickaxe = true;
    private PickaxeGadget pickaxe;

    public override void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("PickaxePickUp"))
        {
            Debug.Log("The gadget is now ready again");
            hasPickaxe = true;
            gadget.gadgetUI.SetFillAmount(0);
            Destroy(coll.gameObject);
        }
        base.OnTriggerEnter(coll);
    }

    public override void UseGadget()
    {
        if (hasPickaxe)
        {
            //pickaxe = (PickaxeGadget)gadget;
            if (Input.GetMouseButton(1))
            {
                //pickaxe.ThrowGadget();
                networkActions.ReaperGadgetAction(true);
                hasPickaxe = false;
                Debug.Log("Do you have it?" + hasPickaxe);
            }
            else
            {
                networkActions.ReaperGadgetAction(false);
                //pickaxe.GadgetAction();
            }
        }
    }

    public void SetMovement(bool status)
    {
        movementEnabled = status;
        aimingEnabled = status;
    }
}
