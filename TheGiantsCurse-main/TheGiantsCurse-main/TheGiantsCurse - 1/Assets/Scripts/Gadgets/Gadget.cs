using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour
{
    public GadgetUIBehaviour gadgetUI;
    protected bool isReady, onCooldown;
    protected float tmpCooldown;

    public virtual void GadgetAction()
    {
        Debug.Log("I should not be here");
    }

    public void StartCooldown(float cooldown)
    {
        gadgetUI.Cooldown(cooldown);
        tmpCooldown = cooldown;
        onCooldown = true;
        Debug.Log("Start cooldown of: " + cooldown);
        //StartCoroutine(Cooldown(cooldown));
    }

    private IEnumerator Cooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        isReady = true;
        Debug.Log("Gadget is ready!");
    }

    protected virtual void Update()
    {
        if (onCooldown)
        {
            tmpCooldown -= Time.deltaTime;

            if (tmpCooldown <= 0)
            {
                tmpCooldown = 0;
                isReady = true;
                onCooldown = false;
                Debug.Log("Gadget is ready!");
            }
        }
    }

    public void ReduceCooldown(float amount)
    {
        gadgetUI.ReduceCooldown(amount);

        if (tmpCooldown > 0)
        {
            tmpCooldown -= amount;

            if(tmpCooldown <= 0)
            {
                tmpCooldown = 0;
                isReady = true;
                onCooldown = false;
                Debug.Log("Gadget is ready!");
            }
        }
    }
}
