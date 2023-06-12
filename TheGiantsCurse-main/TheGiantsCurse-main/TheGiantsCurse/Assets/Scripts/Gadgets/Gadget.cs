using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour
{
    protected bool isReady;

    public virtual void GadgetAction()
    {
        Debug.Log("I should not be here");
    }

    public void StartCooldown(float cooldown)
    {
        StartCoroutine(Cooldown(cooldown));
    }

    private IEnumerator Cooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        isReady = true;
        Debug.Log("Gadget is ready!");
    }
}
