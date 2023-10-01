using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_InputField joinCodeInputField;

    [SerializeField] private Button startButton;
    [SerializeField] private Button joinButton;

    public static StartMenuScript instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        if (instance == null)
        {
            instance = this;
        }
    }

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player id: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }


    public void StartHost()
    {
        SetButtonsActive(false);
        ServerManager.instance.StartHost();
    }

    public void StartServer()
    {
        SetButtonsActive(false);
        ServerManager.instance.StartServer();
        
    }

    public void StartClient()
    {
        SetButtonsActive(false);
        ClientManager.instance.StartClient(joinCodeInputField.text);
    }

    public void SetButtonsActive(bool status)
    {
        joinButton.interactable = status;
        startButton.interactable = status;
    }
}
