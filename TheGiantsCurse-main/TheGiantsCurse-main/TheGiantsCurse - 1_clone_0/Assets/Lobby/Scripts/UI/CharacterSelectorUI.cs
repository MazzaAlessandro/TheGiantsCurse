using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorUI : MonoBehaviour{
    public GameObject optionPrefab;

    public Transform selectedCharacter;
    public Transform prevCharacter;
    
    private void Start(){
        foreach(Character c in GameManager.instance.characters){
            GameObject option = Instantiate(optionPrefab, transform);
            Button button = option.GetComponent<Button>();

            button.onClick.AddListener(() => {
                GameManager.instance.SetCharacter(c);
                if(selectedCharacter != null){
                    prevCharacter = selectedCharacter;
                }
                selectedCharacter = option.transform;
            });

            Image image = option.GetComponentInChildren<Image>();
            image.sprite = c.icon;

        }
    }
}
