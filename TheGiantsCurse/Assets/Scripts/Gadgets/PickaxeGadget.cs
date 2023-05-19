using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeGadget : Gadget
{
    [SerializeField] private PickaxeBehaviour pickaxePrefab;
    [SerializeField] private GameObject meleePickaxe;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float throwSpeed = 15f;
    [SerializeField] private float rotationSpeed = 15f;
    private float countdown;

    private PickaxeBehaviour paInstance;
    

    private bool isReady;
    // Start is called before the first frame update
    void Start()
    {
        meleePickaxe.SetActive(false);
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
            MeleeAttack();
            countdown = 0f;
            isReady = false;
        }
        else
            Debug.Log("Gadget on cooldown");
    }

    public void ThrowGadget()
    {
        Debug.Log("Throwing the Pickaxe");
        paInstance = Instantiate(pickaxePrefab, transform);
        paInstance.SetThrown();
        paInstance.transform.localPosition = Vector3.forward;
        paInstance.transform.SetParent(null);
        paInstance.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwSpeed, ForceMode.Impulse);
        paInstance.gameObject.GetComponent<Rigidbody>().AddTorque(transform.up * rotationSpeed, ForceMode.Impulse);
    }

    private void MeleeAttack()
    {
        Debug.Log("Activate Pickaxe Gadget! Cooldown: " + cooldown);
        //meleeInstance = Instantiate(meleePickaxe, transform);
        meleePickaxe.SetActive(true);
        meleePickaxe.transform.localPosition = Vector3.zero;
        meleePickaxe.GetComponent<Animator>().SetTrigger("Attack");
    }

    public void AttackEnd()
    {
        meleePickaxe.SetActive(false);
        GetComponentInParent<ReaperController>().EnableMovement();
    }
}
