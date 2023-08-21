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
    public bool leapReady, clubReady, boulderReady, doingAction;


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
        switch (ability)
        {
            case 0:
                ClubAttack();
                break;
            case 1:
                Leap();
                break;
            case 2:
                ThrowBoulder();
                break;
            default:
                Debug.LogError($"Giant ability number {ability} does not exist");
                break;
        }
    }

    #endregion


    //NOTED: I'M JUST BUILDING UP THE SKELETON OF THE NETWORK ACTIONS, THESE METHODS ARE EMPTY AND ALL THE WRITTEN LINES ARE THERE JUST TO PREVENT ANY COMPILATION ERROR
    //The methods mimic how Giant Controller does the things locally
    #region Club ability

    private void ClubAttack()
    {

    }

    public void ClubAttackEnd()
    {

    }

    private IEnumerator ClubRecharge()
    {
        yield return new WaitForSeconds(clubCooldown);
    }

    #endregion

    #region Boulder ability

    private void ThrowBoulder()
    {

    }

    public void Boulder()
    {

    }

    private IEnumerator BoulderTravel()
    {
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator BoulderRecharge()
    {
        yield return new WaitForSeconds(boulderCooldown);
        
        
    }

    #endregion

    #region Leap ability

    private void Leap()
    {

    }

    private IEnumerator Jump()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator LeapAction()
    {
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator LeapEnd()
    {
        yield return new WaitForSeconds(1f);
    }

    private void LandingCrash()
    {

    }

    private IEnumerator LeapRecharge()
    {
        yield return new WaitForSeconds(leapCooldown);
    }

    #endregion
}
