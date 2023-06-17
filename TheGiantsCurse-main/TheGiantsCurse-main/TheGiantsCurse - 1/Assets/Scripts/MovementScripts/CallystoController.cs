using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallystoController : PlayerController
{
    
    public override void ChargeArrow()
    {
        if (!fullCharge)
        {
            arrowCharge += Time.deltaTime * 0.66f;
            if (arrowCharge >= chargeCap)
            {
                arrowCharge = chargeCap;
                Debug.Log("Maximum charge reached at: " + arrowCharge);
                fullCharge = true;
            }
        }
    }

    public override void Shoot()
    {
        float finalArrowSpeed = arrowSpeed * arrowCharge;
        arrowCharge = chargeStart;
        arrowCounter--;
        arrowUI.UpdateArrowNumber(arrowCounter);
        Debug.Log("Arrow Speed is: " + finalArrowSpeed + " and remaining arrows are: " + arrowCounter);
        var force = transform.TransformDirection(Vector3.forward);
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint);
        currentArrow.transform.localPosition = Vector3.zero;
        if (fullCharge)
            currentArrow.MakeFireArrow();
        if (ropedArrow)
        {
            currentArrow.MakeRoped();
            arrowUI.SetRopeImage(false);
            aimingEnabled = false;
            movementEnabled = false;
        }
        currentArrow.Shoot(transform.forward * finalArrowSpeed);
        currentArrow.SetOwner(this.gameObject);
        //currentArrow.Shoot(transform.forward, finalArrowSpeed);
        currentArrow = null;
        fullCharge = false;
        ropedArrow = false;
        if (arrowCounter > 0)
            Reload();
    }

    protected override IEnumerator NewLevel()
    {
        
        yield return new WaitForSeconds(2f);
        Debug.Log("Set up camera");
        cameraInstance = Instantiate(cameraPrefab, null);
        mainCamera = cameraInstance.GetComponentInChildren<Camera>();
        cameraInstance.GetComponent<CameraFollow>().ChangeFollow(this.gameObject);
        GameObject.FindWithTag("tmpCam").SetActive(false);
        gadget.GetComponent<LanternGadget>().SetGlobalLight(GameObject.FindWithTag("GlobalLight").GetComponent<Light>());
        transform.position = new Vector3(spawnPoint.transform.position.x, 10, spawnPoint.transform.position.z);
        //mainCamera = FindObjectOfType<Camera>();
        rb.useGravity = true;
        StartCoroutine(MovementEnabler());
    }
}
