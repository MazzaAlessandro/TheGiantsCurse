using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Database", menuName = "Characters/Database")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private Characters[] characters = new Characters[0];

    public Characters[] GetAllCharacters() => characters;

    public Characters GetCharacterById(int id)
    {
        foreach (var character in characters)
        {
            if (character.Id == id)
            {
                return character;
            }
        }

        return null;
    }

    public bool IsValidCharacterId(int id)
    {
        return characters.Any(x => x.Id == id);
    }
}
