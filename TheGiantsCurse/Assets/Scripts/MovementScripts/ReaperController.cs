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
            Destroy(coll.gameObject);
        }
        base.OnTriggerEnter(coll);
    }

    public override void UseGadget()
    {
        if (hasPickaxe)
        {
            pickaxe = (PickaxeGadget)gadget;
            if (Input.GetMouseButton(1))
            {
                pickaxe.ThrowGadget();
                hasPickaxe = false;
                Debug.Log("Do you have it?" + hasPickaxe);
            }
            else
            {
                movementEnabled = false;
                aimingEnabled = false;
                pickaxe.GadgetAction();
            }
        }
    }

    public void EnableMovement()
    {
        movementEnabled = true;
        aimingEnabled = true;
    }
}
