using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private Button button;

    private CharacterSelectionMenu characterSelect;

    public Characters Character { get; private set; }
    public bool IsDisabled { get; private set; }

    public void SetCharacter(CharacterSelectionMenu characterSelect, Characters character)
    {
        iconImage.sprite = character.Icon;

        this.characterSelect = characterSelect;

        Character = character;
    }

    public void SelectCharacter()
    {
        characterSelect.Select(Character);
    }

    public void SetDisabled()
    {
        Debug.Log("Disabled");
        IsDisabled = true;
        disabledOverlay.SetActive(true);
        button.interactable = false;
    }
}
