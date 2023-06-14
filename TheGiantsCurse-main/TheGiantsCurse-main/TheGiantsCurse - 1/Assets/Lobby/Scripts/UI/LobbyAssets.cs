using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAssets : MonoBehaviour {

    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Sprite firstSprite;
    [SerializeField] private Sprite secondSprite;
    [SerializeField] private Sprite thirdSprite;
    [SerializeField] private Sprite forthSprite;
    [SerializeField] private Sprite fifthSprite;


    private void Awake() {
        Instance = this;
    }

    public Sprite GetSprite(LobbyManager.PlayerCharacter playerCharacter) {
        switch (playerCharacter) {
            default:
            case LobbyManager.PlayerCharacter.First:   return firstSprite;
            case LobbyManager.PlayerCharacter.Second:    return secondSprite;
            case LobbyManager.PlayerCharacter.Third:   return thirdSprite;
            case LobbyManager.PlayerCharacter.Forth:   return forthSprite;
            case LobbyManager.PlayerCharacter.Fifth:   return fifthSprite;
        }
    }

}