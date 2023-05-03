using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour
{
    public virtual void GadgetAction()
    {
        Debug.Log("I should not be here");
    }
}
