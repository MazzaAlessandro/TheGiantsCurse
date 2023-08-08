using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class StartMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_InputField joinCodeInputField;

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player id: {AuthenticationService.Instance.PlayerId}");
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }


    public void StartHost()
    {
        ServerManager.instance.StartHost();
    }

    public void StartServer()
    {
        ServerManager.instance.StartServer();
        
    }

    public void StartClient()
    {
        ClientManager.instance.StartClient(joinCodeInputField.text);
    }
}
