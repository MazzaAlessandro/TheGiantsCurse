using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GiantNetworkActions : NetworkBehaviour
{
    [SerializeField] private float leapCooldown = 5f;
    [SerializeField] private float clubCooldown = 2f;
    [SerializeField] private float boulderCooldown = 10f;
    [SerializeField] private float boulderSpeed = 20f;
    [SerializeField] private float leapCrashRange = 8f;

    [SerializeField] private GameObject club;
    [SerializeField] private GameObject boulder;
    [SerializeField] private GameObject leapLandingArea;

    private GameObject boulderInstance, leapLandingInstance;

    //This set of bools are public because Giant Controller will need to check on them, replacing their own bools
    public bool leapReady, clubReady, boulderReady, doingAction, canMove, onAir;

    public GadgetUIBehaviour Ability1UI;
    public GadgetUIBehaviour Ability2UI;
    public GadgetUIBehaviour Ability3UI;

    [SerializeField] private Animator animator;

    public void SetUp()
    {
        leapReady = true;
        clubReady = true;
        boulderReady = true;
        doingAction = false;
        canMove = true;
        onAir = false;
    }

    public void GetOwnership()
    {
        ChangeOwnershipServerRpc(NetworkManager.LocalClientId);
    }

    [ServerRpc(RequireOwnership =false)]
    private void ChangeOwnershipServerRpc(ulong client)
    {
        GetComponent<NetworkObject>().ChangeOwnership(client);
    }

    #region Test structure

    //Just testing if it works
    public void Test(int num)
    {
        Debug.Log($"Sent by: {num}");
        TestServerRpc(num);
    }

    [ServerRpc]
    private void TestServerRpc(int num)
    {
        Debug.Log($"Sent on server by: {num}");
        TestClientRpc(num);

    }

    [ClientRpc]
    private void TestClientRpc(int num)
    {
        Debug.Log($"Sent on client by: {num}");
        TestMethod(num);

    }

    private void TestMethod(int num)
    {
        Debug.Log($"Executed message sent by: {num}");
    }

    #endregion

    #region Abilities activation

    public void Ability(int ability)
    {
        AbilityServerRpc(ability);
    }

    [ServerRpc]
    private void AbilityServerRpc(int ability)
    {
        AbilityClientRpc(ability);
    }

    [ClientRpc]
    private void AbilityClientRpc(int ability)
    {
        LocalAbility(ability);
    }

    private void LocalAbility(int ability)
    {
        Debug.Log($"Activating ability {ability}");
        if (!doingAction)
        {
            switch (ability)
            {
                case 0:
                    if (clubReady)
                        ClubAttack();
                    break;
                case 1:
                    if (leapReady)
                        Leap();
                    break;
                case 2:
                    if (boulderReady)
                        ThrowBoulder();
                    break;
                default:
                    Debug.LogError($"Giant ability number {ability} does not exist");
                    break;
            }
        }
        
    }

    #endregion


    //NOTED: I'M JUST BUILDING UP THE SKELETON OF THE NETWORK ACTIONS, THESE METHODS ARE EMPTY AND ALL THE WRITTEN LINES ARE THERE JUST TO PREVENT ANY COMPILATION ERROR
    //The methods mimic how Giant Controller does the things locally
    #region Club ability

    private void ClubAttack()
    {
        Ability1UI.SetFillAmount(1);
        Debug.Log("Activate Club");
        canMove = false;
        club.SetActive(true);
        animator.SetTrigger("attack1");
        club.transform.localPosition = new Vector3(0, -0.4f, 0.6f);
        club.GetComponent<Animator>().SetTrigger("Attack");
        clubReady = false;
        doingAction = true;
    }

    public void ClubAttackEnd()
    {
        canMove = true;
        doingAction = false;
        club.SetActive(false);
        Ability1UI.Cooldown(clubCooldown);
        StartCoroutine(ClubRecharge());
    }

    private IEnumerator ClubRecharge()
    {
        yield return new WaitForSeconds(clubCooldown);
        clubReady = true;
    }

    #endregion

    #region Boulder ability

    private void ThrowBoulder()
    {
        Debug.Log("Activate Boulder");
        canMove = false;
        boulderReady = false;
        doingAction = true;
        animator.SetTrigger("attack2");
    }

    public void Boulder()
    {
        boulderInstance = Instantiate(boulder, transform);
        boulderInstance.transform.localPosition = new Vector3(0, 1, 3f);
        boulderInstance.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        boulderInstance.transform.SetParent(null);
        boulderInstance.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * boulderSpeed, ForceMode.Impulse);
        boulderInstance.gameObject.GetComponent<Rigidbody>().AddTorque(transform.right * 5, ForceMode.Impulse);
        Ability3UI.SetFillAmount(1);
        StartCoroutine(BoulderTravel());
    }

    private IEnumerator BoulderTravel()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        doingAction = false;
        yield return new WaitForSeconds(4f);
        Destroy(boulderInstance);
        Ability3UI.Cooldown(boulderCooldown);
        StartCoroutine(BoulderRecharge());
    }

    private IEnumerator BoulderRecharge()
    {
        yield return new WaitForSeconds(boulderCooldown);
        Debug.Log("The boulder is now ready again");
        boulderReady = true;
    }

    #endregion

    #region Leap ability

    private void Leap()
    {
        doingAction = true;
        leapReady = false;
        StartCoroutine(Jump());
    }

    private IEnumerator Jump()
    {
        Ability2UI.SetFillAmount(1);

        canMove = false;

        float elapsedTime = 0f;
        float time = 0.25f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(transform.position.x, 30, transform.position.z);

        while (elapsedTime < time)
        {
            if (IsOwner)
                transform.position = Vector3.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        leapLandingInstance = Instantiate(leapLandingArea, transform);
        leapLandingInstance.transform.SetParent(null);
        leapLandingInstance.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);

        canMove = true;

        StartCoroutine(LeapAction());
    }

    private IEnumerator LeapAction()
    {
        onAir = true;

        float elapsedTime = 0f;
        float time = 5f;

        while (elapsedTime < time)
        {
            leapLandingInstance.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(LeapEnd());
    }

    public IEnumerator LeapEnd()
    {
        canMove = false;

        float elapsedTime = 0f;
        float time = 0.25f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(transform.position.x, -0.2f, transform.position.z);

        while (elapsedTime < time)
        {
            if (IsOwner) 
                transform.position = Vector3.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        LandingCrash();

        onAir = false;
        canMove = true;

        Destroy(leapLandingInstance);
        Debug.Log("You should land here");
        doingAction = false;
        Ability2UI.Cooldown(leapCooldown);
        StartCoroutine(LeapRecharge());
    }

    private void LandingCrash()
    {
        GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>().SmallShake(0.5f, 1f);

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, leapCrashRange);
        foreach (var objectHit in objectsInRange)
        {
            if (objectHit.CompareTag("Player"))
            {
                objectHit.GetComponent<PlayerController>().Stun(1.2f);
            }
        }
    }

    private IEnumerator LeapRecharge()
    {
        yield return new WaitForSeconds(leapCooldown);
        leapReady = true;
    }

    #endregion
}
